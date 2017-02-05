using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Fds.IFAPI;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace IF_FMS
{
    /// <summary>
    /// FMS for IF
    /// </summary>
    public partial class FMS : UserControl
    {

        public static bool textFieldFocused = false;

        private IFConnect.IFConnectorClient client;
        public IFConnect.IFConnectorClient Client {
            get { return client; }
            set { client = value; }
        }

        

        private APIAircraftState pAircraftState = new APIAircraftState();
        public bool autoFplDirectActive = false;

        public FMS()
        {
            InitializeComponent();
        }

        #region Flight Plan Classes
        public class fplDetails : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private string pWpt;
            public string WaypointName {
                get { return pWpt; }
                set
                {
                    pWpt = value;
                    this.NotifyPropertyChanged("WaypointName");
                }
            }

            private double pAlt;
            public double Altitude {
                get { return pAlt; }
                set
                {
                    pAlt = value;
                    this.NotifyPropertyChanged("Altitude");
                }
            }

            private double pSpeed;
            public double Airspeed
            {
                get { return pSpeed; }
                set
                {
                    pSpeed = value;
                    this.NotifyPropertyChanged("Airspeed");
                }
            }

            private void NotifyPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private bool pManualRetrieveFPL = false;

        public class customFlightplan
        {
            private System.Collections.ObjectModel.ObservableCollection<fplDetails> pWaypoints;
            public System.Collections.ObjectModel.ObservableCollection<fplDetails> waypoints {
                get {
                    if (pWaypoints == null)
                    {
                        pWaypoints = new System.Collections.ObjectModel.ObservableCollection<fplDetails>();
                    }
                    return pWaypoints;
                 }
                
            }

            public void addWaypoint(String wpt, double altitude, double airspeed)
            {
                //if (waypoints == null) { waypoints = new System.Collections.ObjectModel.ObservableCollection<fplDetails>(); }
                waypoints.Add(new fplDetails { WaypointName = wpt, Altitude = altitude, Airspeed = airspeed });
            }
        }

        #endregion

        private customFlightplan pCustomFpl;
        public customFlightplan CustomFPL
        {
            get {
                if (pCustomFpl == null)
                {
                    pCustomFpl = new customFlightplan();
                }
                return pCustomFpl;
            }
            set { pCustomFpl = value; }
        }
        
        public System.Collections.ObjectModel.ObservableCollection<fplDetails> CustomFplWaypoints
        {
            get { return CustomFPL.waypoints; }
        }

        private bool pSettingFpl;
        public void setCustomFlightPlan()
        {
            pSettingFpl = true;
            var items = CustomFPL.waypoints;
            client.ExecuteCommand("Commands.FlightPlan.Clear");
            client.ExecuteCommand("Commands.FlightPlan.AddWaypoints", items.Select(x => new CallParameter { Name = "WPT", Value = x.WaypointName }).ToArray());
        }

        #region GUI Handlers
        private void btnGetFpl_Click(object sender, RoutedEventArgs e)
        {
            pSettingFpl = false;
            client.ExecuteCommand("FlightPlan.GetFlightPlan");
        }

        private void btnSetFpl_Click(object sender, RoutedEventArgs e)
        {
            setCustomFlightPlan();
        }

        private void btnClrFpl_Click(object sender, RoutedEventArgs e)
        {
            CustomFPL.waypoints.Clear();
            client.ExecuteCommand("Commands.FlightPlan.Clear");
        }


        private void btnInitFlightDir_Click(object sender, RoutedEventArgs e)
        {
            if (!autoFplDirectActive) //AutoNAV not active. Turn it on.
            {
                getAPState();
                if (pFplState==null || pFplState.fpl == null)
                {
                    MessageBox.Show("Must get or set FPL first.");
                }
                else if (pFplState.fpl.Waypoints.Length > 0)
                {
                    //updateAutoNav(pAircraftState);
                    autoFplDirectActive = true;
                    lblFmsState.Content = "AutoNAV Enabled";
                    lblFmsState.Foreground = System.Windows.Media.Brushes.DarkGreen;
                    btnInitFlightDir.Content = "Disable AutoNAV";
                }
            }
            else //AutoNav running. Turn it off.
            {
                autoFplDirectActive = false;
                lblFmsState.Content = "AutoNAV Disabled";
                lblFmsState.Foreground = System.Windows.Media.Brushes.Red;
                btnInitFlightDir.Content = "Enable AutoNAV";
            }
        }

        private void btnDisFlightDir_Click(object sender, RoutedEventArgs e)
        {
            autoFplDirectActive = false;
            lblFmsState.Content = "AutoNAV Disabled";
            lblFmsState.Foreground = System.Windows.Media.Brushes.Red;
           // pFplState = null;
        }
        #endregion

        public void fplReceived(APIFlightPlan fpl)
        {
            if (!pSettingFpl)
            {
                FPLState = new FMS.flightPlanState();
                FPLState.fpl = fpl;
                pManualRetrieveFPL = true;
                CustomFPL.waypoints.Clear(); //= new customFlightplan();

                foreach (APIWaypoint wpt in fpl.Waypoints)
                {
                    if (wpt.Name != "WPT")
                    {
                        CustomFPL.addWaypoint(wpt.Name, -1, -1);
                    }
                }

                //          this.dgFpl.ItemsSource = CustomFPL.waypoints;
                dgFpl.Items.Refresh();
                pFplState.fplDetails = CustomFPL;
                pManualRetrieveFPL = false;
            }
        }

        #region AutoNAV
        public class flightPlanState
        {
            public APIFlightPlan fpl; //Entire FPL
            public customFlightplan fplDetails;
            public int legIndex; //Index of leg (really wpt index)
            public APIWaypoint prevWpt; //Last waypoint
            public APIWaypoint nextWpt; //Next waypoint
            public APIWaypoint dest; //Destination
            public double distToNextWpt;
            public double hdgToNextWpt;
            public double distToDest;
            public double thisAltitude;
            public double nextAltitude;
            public double thisSpeed;
            public double nextSpeed;
            public event EventHandler fplStateUpdate;
        }

        private flightPlanState pFplState;
        public flightPlanState FPLState
        {
            get { return pFplState; }
            set { pFplState = value; }
        }

        public void updateAutoNav(APIAircraftState acState)
        {
            if (pFplState == null)
            {
                client.ExecuteCommand("FlightPlan.GetFlightPlan");
            }

            //Set initial next waypoint as first waypoint
            if (pFplState.nextWpt == null)
            {
                pFplState.legIndex = 0;
                pFplState.nextWpt = pFplState.fpl.Waypoints[1];
                pFplState.dest = pFplState.fpl.Waypoints.Last();
                pFplState.nextAltitude = pFplState.fplDetails.waypoints[pFplState.legIndex + 1].Altitude;
                pFplState.thisSpeed = pFplState.fplDetails.waypoints[pFplState.legIndex].Airspeed;
                pFplState.nextSpeed = pFplState.fplDetails.waypoints[pFplState.legIndex + 1].Airspeed;
            }

            //Get dist to next wpt
            pFplState.distToNextWpt = getDistToWaypoint(acState.Location, pFplState.nextWpt);

            //If dist<2nm, go to next wpt
            if (pFplState.distToNextWpt < 2)
            {
                pFplState.legIndex++; //next leg
                pFplState.prevWpt = pFplState.nextWpt; //current wpt is not prev wpt
                if (pFplState.legIndex >= pFplState.fpl.Waypoints.Count())
                {
                    //We hit the destination!
                    lblNextWpt.Content = "Destination Reached!";
                    lblDist2Next.Content = "";
                    lblHdg2Next.Content = "";
                    autoFplDirectActive = false;
                    return;
                }
                pFplState.nextWpt = pFplState.fpl.Waypoints[pFplState.legIndex]; //target next wpt
                pFplState.nextAltitude = pFplState.fplDetails.waypoints[pFplState.legIndex].Altitude;
                pFplState.thisSpeed = pFplState.fplDetails.waypoints[pFplState.legIndex - 1].Airspeed;
                pFplState.nextSpeed = pFplState.fplDetails.waypoints[pFplState.legIndex].Airspeed;
                //Get dist to new next wpt
                pFplState.distToNextWpt = getDistToWaypoint(acState.Location, pFplState.nextWpt);
            }

            lblNextWpt.Content = pFplState.nextWpt.Name;
            lblDist2Next.Content = String.Format("{0:0.000}", pFplState.distToNextWpt);
            lblAirspeedSet.Content = String.Format("{0:0.000}", pFplState.thisSpeed);
            lblAltitudeSet.Content = pFplState.nextAltitude.ToString();

            //Adjust heading for magnetic declination
            double declination = acState.HeadingTrue - acState.HeadingMagnetic;

            //Get heading to next
            pFplState.hdgToNextWpt = getHeadingToWaypoint(acState.Location, pFplState.nextWpt) - declination;
            lblHdg2Next.Content = String.Format("{0:0.000}", pFplState.hdgToNextWpt);

            //Calculate VS to hit target altitude
            if (pFplState.nextAltitude > 0)
            {
                //double vs = calcVs(acState.AltitudeMSL, pFplState.nextAltitude, acState.GroundSpeedKts, pFplState.distToNextWpt);
                double vs = 0;
                var airspeed = pFplState.thisSpeed;
                if (airspeed > 0 && airspeed <= 60)
                {
                    vs = 700;
                }
                else if (airspeed > 60 && airspeed <= 160)
                {
                    vs = 1100;
                }
                else if (airspeed > 160 && airspeed <= 225)
                {
                    vs = 1500;
                }
                else if (airspeed > 225 && airspeed <= 285)
                {
                    vs = 1900;
                }
                else if (airspeed > 285 && airspeed <= 320)
                {
                    vs = 2200;
                }
                else
                {
                    vs = 2300;
                }

                lblVsSet.Content = string.Format("{0:0.000}", vs);
                //Adjust AutoPilot
                setAutoPilotParams(pFplState.nextAltitude, pFplState.hdgToNextWpt, vs, pFplState.nextSpeed);
            }
            else
            {
                //Adjust AutoPilot
                setAutoPilotParams(pFplState.nextAltitude, pFplState.hdgToNextWpt, 999999, pFplState.nextSpeed);
            }

            ////Dont think we need this
            //APIWaypoint closestWpt = pFplState.fpl.Waypoints.First();
            //foreach (APIWaypoint  wpt in pFplState.fpl.Waypoints)
            //{
            //   double distToClosest = getDistToWaypoint(acState.Location, closestWpt);
            //   if (getDistToWaypoint(acState.Location,wpt) < distToClosest)
            //    {
            //        closestWpt = wpt;
            //    }
            //}


        }

        #endregion

        #region "ATC autopilot"
        private void btnEnaAtcLog_Click(object sender, RoutedEventArgs e)
        {
            //Start receiving ATC messages
            client.ExecuteCommand("Live.EnableATCMessageNotification");
        }

        private void logMessage(string msg)
        {
            if(chkFilterCallsignOnly.IsChecked==true && msg.Contains(Callsign))
            {
                txtAtcLog.AppendText(msg + "\n");
                txtAtcLog.ScrollToEnd();
            }else if (chkFilterCallsignOnly.IsChecked == false)
            {
                txtAtcLog.AppendText(msg + "\n");
                txtAtcLog.ScrollToEnd();
            }
        }

        private void chkEnableAtcAutopilot_Checked(object sender, RoutedEventArgs e)
        {
            if (chkEnableAtcAutopilot.IsChecked == true)
            {
                //Start receiving ATC messages
                client.ExecuteCommand("Live.EnableATCMessageNotification");
            }
            if ((chkEnableAtcAutopilot.IsChecked == true) && (txtCurrentCallsign.Text == null || txtCurrentCallsign.Text == ""))
            {
                MessageBox.Show("Must enter your current callsign or this will not work!");
                chkEnableAtcAutopilot.IsChecked = false;
            }
        }
        
        public string Callsign
        {
            get { return txtCurrentCallsign.Text; }
            set { txtCurrentCallsign.Text = value; }
        }
        
        public bool? AtcControlledAutopilotEnabled {
            get { return chkEnableAtcAutopilot.IsChecked; }
            set { chkEnableAtcAutopilot.IsChecked = value; } }
        public void handleAtcMessage(APIATCMessage AtcMsg, APIAircraftState acState)
        {

            logMessage(AtcMsg.Message);
            //Only act on messages received by us that are directed at us.
            if (AtcControlledAutopilotEnabled==true && AtcMsg.Received && AtcMsg.Message.Contains(Callsign))
            {
                string msg = AtcMsg.Message.ToLower(); //Just to make things easier

                //Heading
                if (msg.Contains("heading"))
                {
                    Match mtch = Regex.Match(msg, "heading\\W+(?<hdg>\\d+)");
                    if (mtch.Success) {
                        string sHdg = mtch.Groups["hdg"].Value;
                        double hdg = Convert.ToDouble(sHdg);
                        setHeading(hdg);
                    }
                }

                //Altitude
                if (msg.Contains("descend") || msg.Contains("climb"))
                {
                    Match mtch = Regex.Match(msg, "maintain\\W+(?<fl>FL)*(?<alt>\\d+,*\\d+)(ft)*");
                    if (mtch.Success)
                    {
                        double alt = 1000.0;
                        if (mtch.Groups["fl"].Success) //Convert flight level
                        {
                            alt = Convert.ToDouble(mtch.Groups["alt"].Value) * 100;
                        }
                        else //Else it is just in ft, but remove comma
                        {
                            alt = Convert.ToDouble(mtch.Groups["alt"].Value.Replace(",",""));
                        }
                        if (msg.Contains("descend"))
                        {
                            setAltitude(alt, -1800);
                        }
                        else { setAltitude(alt, 1800); }
                    }
                }

                //Speed
                if (msg.Contains("kts"))
                {
                    Match mtch = Regex.Match(msg, "\\W+(?<speed>\\d+)kts");
                    if (mtch.Success)
                    {
                        double speed = Convert.ToDouble(mtch.Groups["speed"].Value);
                        if(msg.Contains("do not exceed")) { speed -= 5; }
                        setSpeed(speed);
                        if (acState.IndicatedAirspeedKts > (speed + 30))
                        {
                            client.ExecuteCommand("Commands.Spoilers");
                        }
                    }
                }

                //Acknowledge ATC
                client.ExecuteCommand("Commands.ATCEntry2");
                if (msg.Contains("contact") && !msg.Contains("exit runway"))
                {
                    //contact next freq if handed off (but not if exiting runway to contact gnd)
                    client.ExecuteCommand("Commands.ATCEntry2"); //send & switch on second menu
                }
            }

            
            
        }

        #endregion

        #region "Autopilot settings"
        private bool APUpdateReceived = false;
        private APIAutopilotState pAPState = new APIAutopilotState();
        public APIAutopilotState APState
        {
            get
            {
                getAPState();
                return pAPState;
            }
            set { pAPState = value; APUpdateReceived = true; }
        }

        private void getAPState()
        {
            APUpdateReceived = false;
            client.ExecuteCommand("Autopilot.GetState");
        }

        private void setHeading(double heading)
        {
            if (APState.TargetHeading != heading) { client.ExecuteCommand("Commands.Autopilot.SetHeading", new CallParameter[] { new CallParameter { Value = heading.ToString(CultureInfo.InvariantCulture.NumberFormat) } }); }
            if (!APState.EnableHeading) { client.ExecuteCommand("Commands.Autopilot.SetHeadingState", new CallParameter[] { new CallParameter { Value = "True" } }); }
        }

        private void setAltitude(double altitude, double vs)
        {
            if(APState.TargetAltitude != altitude) { client.ExecuteCommand("Commands.Autopilot.SetAltitude", new CallParameter[] { new CallParameter { Value = altitude.ToString(CultureInfo.InvariantCulture.NumberFormat) } }); }
            if(!APState.EnableAltitude) { client.ExecuteCommand("Commands.Autopilot.SetAltitudeState", new CallParameter[] { new CallParameter { Value = "True" } });}
            if(APState.TargetClimbRate!= vs) { client.ExecuteCommand("Commands.Autopilot.SetVS", new CallParameter[] { new CallParameter { Value = vs.ToString(CultureInfo.InvariantCulture.NumberFormat) } });}
            //client.ExecuteCommand("Commands.Autopilot.SetVSState", new CallParameter[] { new CallParameter { Value = "True" } });
        }

        private void setSpeed(double speed)
        {
            if (APState.TargetSpeed != speed) { client.ExecuteCommand("Commands.Autopilot.SetSpeed", new CallParameter[] { new CallParameter { Value = speed.ToString(CultureInfo.InvariantCulture.NumberFormat) } }); }
            if (!APState.EnableSpeed){ client.ExecuteCommand("Commands.Autopilot.SetSpeedState", new CallParameter[] { new CallParameter { Value = "True" } }); }
        }

        private void setAutoPilotParams(double altitude, double heading, double vs, double speed)
        {
            getAPState();
            //Send parameters
            if (speed > 0 && APState.TargetSpeed!=speed) { client.ExecuteCommand("Commands.Autopilot.SetSpeed", new CallParameter[] { new CallParameter { Value = speed.ToString(CultureInfo.InvariantCulture.NumberFormat) } }); }
            if (altitude > 0 && pAPState.TargetAltitude != altitude) { client.ExecuteCommand("Commands.Autopilot.SetAltitude", new CallParameter[] { new CallParameter { Value = altitude.ToString(CultureInfo.InvariantCulture.NumberFormat) } }); }
            if (vs < 999999 && pAPState.TargetClimbRate != vs) { client.ExecuteCommand("Commands.Autopilot.SetVS", new CallParameter[] { new CallParameter { Value = vs.ToString(CultureInfo.InvariantCulture.NumberFormat) } }); }
            if (pAPState.TargetHeading != heading) { client.ExecuteCommand("Commands.Autopilot.SetHeading", new CallParameter[] { new CallParameter { Value = heading.ToString(CultureInfo.InvariantCulture.NumberFormat) } }); }
            //Activate AP
            if (altitude > 0 && !pAPState.EnableAltitude) { client.ExecuteCommand("Commands.Autopilot.SetAltitudeState", new CallParameter[] { new CallParameter { Value = "True" } }); }
            if (!pAPState.EnableHeading) { client.ExecuteCommand("Commands.Autopilot.SetHeadingState", new CallParameter[] { new CallParameter { Value = "True" } }); }
            if (vs < 999999 && !pAPState.EnableClimbRate) { client.ExecuteCommand("Commands.Autopilot.SetVSState", new CallParameter[] { new CallParameter { Value = "True" } }); }
            if (speed > 0 && !pAPState.EnableSpeed) { client.ExecuteCommand("Commands.Autopilot.SetSpeedState", new CallParameter[] { new CallParameter { Value = "True" } }); }
            //if (appr) { client.ExecuteCommand("Commands.Autopilot.SetApproachModeState", new CallParameter[] { new CallParameter { Value = appr.ToString() } }); }
        }


        #endregion

        #region "Conversion/Calculation Helper functions"
        private double deg2rad(double deg)
        {
            return deg * (Math.PI / 180);
        }

        private double rad2deg(double rad)
        {
            return rad * (180 / Math.PI);
        }

        private double getDistToWaypoint(Coordinate curPos, APIWaypoint nextWpt)
        {
            Coordinate next = new Coordinate();
            next.Latitude = nextWpt.Latitude;
            next.Longitude = nextWpt.Longitude;
            return getDistBtwnPoints(curPos, next);
        }

        private double getDistBtwnPoints(Coordinate curPos, Coordinate nextWpt)
        {
            var R = 3440; // Radius of the earth in nm
            var dLat = deg2rad(nextWpt.Latitude - curPos.Latitude);  // deg2rad below
            var dLon = deg2rad(nextWpt.Longitude - curPos.Longitude);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos(deg2rad(curPos.Latitude)) * Math.Cos(deg2rad(nextWpt.Latitude)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in nm
            return d;
        }

        private double getHeadingToWaypoint(Coordinate curPos, APIWaypoint nextWpt)
        {
            Coordinate next = new Coordinate();
            next.Latitude = nextWpt.Latitude;
            next.Longitude = nextWpt.Longitude;
            return getHeadingToPoint(curPos, next);
        }

        private double getHeadingToPoint(Coordinate point1, Coordinate point2)
        {
            double longitude1 = point1.Longitude;
            double longitude2 = point2.Longitude;
            double latitude1 = deg2rad(point1.Latitude);
            double latitude2 = deg2rad(point2.Latitude);
            double longDiff = deg2rad(longitude2 - longitude1);
            double y = Math.Sin(longDiff) * Math.Cos(latitude2);
            double x = Math.Cos(latitude1) * Math.Sin(latitude2) - Math.Sin(latitude1) * Math.Cos(latitude2) * Math.Cos(longDiff);

            return (rad2deg(Math.Atan2(y, x)) + 360) % 360;
        }

        private double calcVs(double curAlt, double targetAlt, double groundSpeed, double distToNext)
        {
            double vs = 0;
            double altDiff = targetAlt - curAlt; //If target alt is lower, this will be negative
            double estTimeToNext = (1/groundSpeed * distToNext) * 60; //ETA in minutes
            vs = altDiff / estTimeToNext; //VS in FPM
            return vs;
        }

        #endregion


        private void dgFpl_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            textFieldFocused = true;
        }

        private void dgFpl_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            textFieldFocused = false;
        }


        #region "Holding"

        private System.Collections.Generic.List<Coordinate> holdingTrack = new System.Collections.Generic.List<Coordinate>();
        private Coordinate initialHoldStart, leg2start;
        private double initialHdg = 0;
        private double hdgSet = 999;
        private double legLength = 5;
        private bool holdingActive = false;
        private int holdLeg = 0;
        private bool hdgSetComplete = false;
        private bool leftTurns = true;
        private double backBearing = 0;

        public bool HoldingActive { get { return holdingActive; } }

        private void btnHold_Click(object sender, RoutedEventArgs e)
        {
            if (!holdingActive)
            {
                holdingTrack.Clear();
                leftTurns = slider.Value==0?true:false;
                legLength = Double.Parse(txtLegLen.Text);
                initialHoldStart = null;
                initialHdg = -1;
                backBearing = 0;
                holdLeg = 0;
                hdgSet = 999;
                hdgSetComplete = false;
                holdingActive = true;
                btnHold.Content = "END HOLD";
            } else
            {
                holdingActive = false;
                btnHold.Content = "HOLD";
            }
        }

        

        public void performHold(APIAircraftState acState)
        {
            pAircraftState = acState;
            Double degChg = 0;
            switch (holdLeg)
            {
                
                case 0:
                    //Get initial fix and heading
                    if (initialHoldStart == null) { initialHoldStart = pAircraftState.Location; }
                    if (initialHdg == -1) { initialHdg = pAircraftState.HeadingMagnetic; }

                    //turn to back-bearing
                    backBearing = initialHdg < 180 ? initialHdg + 180 : initialHdg - 180;
                    if (lblHoldInfo.Content.ToString() != ("Holding: Turn " + Math.Floor(backBearing).ToString())) { lblHoldInfo.Content = ("Holding: Turn " + Math.Floor(backBearing).ToString()); }

                    if (!hdgSetComplete) { hdgSet = initialHdg; }
                    degChg = 0;
                    while (Math.Abs(hdgSet - Math.Floor(backBearing)) > 5)
                    {
                        if (leftTurns) { hdgSet -= 45; } else { hdgSet += 45; }
                        hdgSet %= 360;
                        setAutoPilotParams(-1, Math.Floor(hdgSet), 999999, -1);
                        degChg = 0;
                        // while degChg < requiredchange then wait
                        System.Threading.Thread.Sleep(15000);
                    }
                    setAutoPilotParams(-1, backBearing, 999999, -1);
                    hdgSetComplete = true;
                    if (Math.Abs(Math.Floor(pAircraftState.HeadingMagnetic) - Math.Floor(backBearing)) < 2)
                    {   //hit backbearing
                        setAutoPilotParams(-1, backBearing, 999999, -1);
                        leg2start = pAircraftState.Location;
                        holdLeg = 1;
                    }
                    break;
                case 1:
                    //maintain back-bearing for legLength
                    if (lblHoldInfo.Content.ToString() != ("Holding: Leg 1")) { lblHoldInfo.Content = "Holding: Leg 1"; }
                    double err = acState.CourseTrue - acState.HeadingTrue;
                    setAutoPilotParams(-1, backBearing - err, 999999, -1);
                    hdgSetComplete = false;
                    if (getDistBtwnPoints(leg2start, pAircraftState.Location) > legLength) { holdLeg = 2; }
                    break;
                case 2:
                    //turn back to initial heading
                    if (lblHoldInfo.Content.ToString() != ("Holding: Turn " + Math.Floor(initialHdg).ToString())) { lblHoldInfo.Content = ("Holding: Turn " + Math.Floor(initialHdg).ToString()); }
                    degChg = 0;
                    while (Math.Abs(hdgSet - Math.Floor(initialHdg)) > 1)
                    {
                        if (leftTurns) { hdgSet -= 90; } else { hdgSet += 90; }
                        hdgSet %= 360;

                        setAutoPilotParams(-1, hdgSet, 999999, -1); degChg = 0;
                        System.Threading.Thread.Sleep(2000);

                    }
                    setAutoPilotParams(-1, initialHdg, 999999, -1);
                    hdgSetComplete = true;
                    if (Math.Abs(Math.Floor(pAircraftState.HeadingMagnetic) - Math.Floor(initialHdg)) < 2)
                    {
                        //hit initial hdg
                        setAutoPilotParams(-1, initialHdg, 999999, -1);
                        //initialHoldStart = pAircraftState.Location;
                        holdLeg = 3;
                    }
                    break;
                case 3:
                    //Maintain initial heading for legLength back to the hold fix
                    if (lblHoldInfo.Content.ToString() != "Holding: To Fix") { lblHoldInfo.Content = "Holding: To Fix"; }
                    hdgSetComplete = false;

                    //if (getDistBtwnPoints(initialHoldStart, pAircraftState.Location) > legLength) { holdLeg = 1; }
                    //Fly to initial Fix
                    double declination = acState.HeadingTrue - acState.HeadingMagnetic;
                    double hdg = getHeadingToPoint(pAircraftState.Location, initialHoldStart) - declination;
                    err = acState.CourseTrue - acState.HeadingTrue;
                    setAutoPilotParams(-1, hdg-err, 999999, -1);
                    if (getDistBtwnPoints(initialHoldStart, pAircraftState.Location) < 0.25) { holdLeg = 0; }
                    break;
                default:
                    break;
            }

            return;
        }

        #endregion

    }
}
