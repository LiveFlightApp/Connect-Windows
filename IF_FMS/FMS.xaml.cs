using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Fds.IFAPI;
using System.ComponentModel;
using System.Globalization;

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

        

        private void setAutoPilotParams(double altitude, double heading, double vs, double speed)
        {
            //Send parameters
            if (speed > 0) { client.ExecuteCommand("Commands.Autopilot.SetSpeed", new CallParameter[] { new CallParameter { Value = speed.ToString(CultureInfo.InvariantCulture.NumberFormat) } }); }
            if (altitude > 0) { client.ExecuteCommand("Commands.Autopilot.SetAltitude", new CallParameter[] { new CallParameter { Value = altitude.ToString(CultureInfo.InvariantCulture.NumberFormat) } }); }
            if (vs < 999999) { client.ExecuteCommand("Commands.Autopilot.SetVS", new CallParameter[] { new CallParameter { Value = vs.ToString(CultureInfo.InvariantCulture.NumberFormat) } }); }
            client.ExecuteCommand("Commands.Autopilot.SetHeading", new CallParameter[] { new CallParameter { Value = heading.ToString(CultureInfo.InvariantCulture.NumberFormat) } });
            //Activate AP
            if (altitude > 0) { client.ExecuteCommand("Commands.Autopilot.SetAltitudeState", new CallParameter[] { new CallParameter { Value = "True" } }); }
            client.ExecuteCommand("Commands.Autopilot.SetHeadingState", new CallParameter[] { new CallParameter { Value = "True" } });
            if (vs < 999999) { client.ExecuteCommand("Commands.Autopilot.SetVSState", new CallParameter[] { new CallParameter { Value = "True" } }); }
            if (speed > 0) { client.ExecuteCommand("Commands.Autopilot.SetSpeedState", new CallParameter[] { new CallParameter { Value = "True" } }); }
            //if (appr) { client.ExecuteCommand("Commands.Autopilot.SetApproachModeState", new CallParameter[] { new CallParameter { Value = appr.ToString() } }); }
        }

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
            double longitude1 = curPos.Longitude;
            double longitude2 = nextWpt.Longitude;
            double latitude1 = deg2rad(curPos.Latitude);
            double latitude2 = deg2rad(nextWpt.Latitude);
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
    }
}
