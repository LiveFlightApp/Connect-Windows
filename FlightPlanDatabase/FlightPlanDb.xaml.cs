using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using IF_FMS;

namespace FlightPlanDatabase
{
    /// <summary>
    /// Interaction logic for FlightPlanDb.xaml
    /// </summary>
    public partial class FlightPlanDb : UserControl
    {
        public event EventHandler FplUpdated;

        // for keyboard events
        public static bool textFieldFocused = false;

        private List<FMS.fplDetails> pFmsFpl;
        public List<FMS.fplDetails> FmsFpl {
            get {
                if (pFmsFpl == null) { pFmsFpl = new List<FMS.fplDetails>(); }
                return pFmsFpl;
            }
            set {
                if (pFmsFpl == null) { pFmsFpl = new List<FMS.fplDetails>(); }
                pFmsFpl = value;
                if (this.FplUpdated != null) { this.FplUpdated(this,null); }
            }
        }

        private Fds.IFAPI.APIFlightPlan pApiFpl;
        public Fds.IFAPI.APIFlightPlan ApiFpl
        {
            get { return pApiFpl; }
            set { pApiFpl = value; }
        }


        public FlightPlanDb()
        {
            InitializeComponent();
            fpdLnk.RequestNavigate += (s, e) => { System.Diagnostics.Process.Start(e.Uri.ToString()); };
        }

        private List<FlightPlanDatabase.ApiDataTypes.FlightPlanSummary> pFplList;

        private void btnGetFplFromFpd_Click(object sender, RoutedEventArgs e)
        {
            FlightPlanDatabase.FpdApi fd = new FlightPlanDatabase.FpdApi();
            List<FlightPlanDatabase.ApiDataTypes.FlightPlanSummary> fpls = new List<FlightPlanDatabase.ApiDataTypes.FlightPlanSummary>();
            try {
                fpls = fd.searchFlightPlans(txtFromICAO.Text, txtDestICAO.Text);
            }catch(Exception ex)
            {
                String exmsg = ex.Message;
                if (ex.Message.Contains(":")) { exmsg = exmsg.Split(':')[1]; }
                if (ex.Message.Contains(")")) { exmsg = exmsg.Split(')')[1]; }
                lblSearchMsg.Content = "Error: " + exmsg;
                return;
            }
            cbFpls.Items.Clear();
            if (fpls == null || fpls.Count < 1)
            {
                lblSearchMsg.Content = "Suitable FPL could not be found.";
                cbFpls.Visibility = Visibility.Collapsed;
            }
            else
            {
                pFplList = new List<FlightPlanDatabase.ApiDataTypes.FlightPlanSummary>();
                pFplList = fpls;
                foreach (FlightPlanDatabase.ApiDataTypes.FlightPlanSummary f in fpls)
                {
                    cbFpls.Items.Add(f.id + " (" + String.Format("{0:0.00}", f.distance) + "nm - " + f.waypoints.ToString() + "wpts)");
                }
                lblSearchMsg.Content = "FPL(s) found. Select below to load.";
                cbFpls.Visibility = Visibility.Visible;
            }
        }

        private void cbFpls_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbFpls.Items.Count > 0 && cbFpls.SelectedValue != null)
            {
                string fplID = cbFpls.SelectedValue.ToString().Split(' ').First();
                loadFpdFplFromId(fplID);
                txtFplId.Text = fplID;
            }
        }

        private void btnLoadFromId_Click(object sender, RoutedEventArgs e)
        {
            if (txtFplId.Text.Length > 0)
            {
                loadFpdFplFromId(txtFplId.Text);
            }
        }

        private void loadFpdFplFromId(string id)
        {
            FlightPlanDatabase.FpdApi fd = new FlightPlanDatabase.FpdApi();
            FlightPlanDatabase.ApiDataTypes.FlightPlanDetails planDetail = fd.getPlan(id);
            //FMSControl.CustomFPL.waypoints.Clear();
            List<FMS.fplDetails> fpl = new List<FMS.fplDetails>();
            pApiFpl = new Fds.IFAPI.APIFlightPlan();
            List<Fds.IFAPI.APIWaypoint> apiWpts = new List<Fds.IFAPI.APIWaypoint>();

            foreach (FlightPlanDatabase.ApiDataTypes.Node wpt in planDetail.route.nodes)
            {
                Fds.IFAPI.APIWaypoint apiWpt = new Fds.IFAPI.APIWaypoint();
                apiWpt.Name = wpt.ident;
                apiWpt.Code = wpt.name;
                apiWpt.Latitude = wpt.lat;
                apiWpt.Longitude = wpt.lon;
                apiWpts.Add(apiWpt);

                FMS.fplDetails n = new FMS.fplDetails();
                n.WaypointName = wpt.ident;
                n.Altitude = wpt.alt;

                //FMSControl.CustomFPL.waypoints.Add(n);
                fpl.Add(n);
            }

            pApiFpl.Waypoints = apiWpts.ToArray();

            FmsFpl = fpl;
        }

        private void txtFromICAO_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = Brushes.Black;
            tb.GotFocus -= txtFromICAO_GotFocus;

            textFieldFocused = true;
        }

        private void txtDestICAO_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = Brushes.Black;
            tb.GotFocus -= txtDestICAO_GotFocus;

            textFieldFocused = true;
        }

        private void txtFromICAO_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == string.Empty)
            {
                tb.Foreground = Brushes.LightGray;
                tb.GotFocus += txtFromICAO_GotFocus;
                tb.Text = "KLAX";
            }

            textFieldFocused = false;
        }

        private void txtDestICAO_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == string.Empty)
            {
                tb.Foreground = Brushes.LightGray;
                tb.GotFocus += txtDestICAO_GotFocus;
                tb.Text = "KSAN";
            }

            textFieldFocused = false;
        }

        private void txtFplId_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.Foreground = Brushes.Black;
            tb.GotFocus -= txtDestICAO_GotFocus;

            textFieldFocused = true;
        }

        private void txtFplId_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == string.Empty)
            {
                tb.Foreground = Brushes.LightGray;
                tb.GotFocus += txtDestICAO_GotFocus;
                tb.Text = "81896";
            }

            textFieldFocused = false;

        }

    }
}
