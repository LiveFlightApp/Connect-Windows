using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IF_FMS
{
    /// <summary>
    /// Interaction logic for AircraftStatus.xaml
    /// </summary>
    public partial class AircraftStatus : UserControl
    {
        public AircraftStatus()
        {
            InitializeComponent();
        }


        public class AircraftStatusForDisplay
        {
            public float AltitudeAGL { get; set; }
            public float AltitudeMSL { get; set; }
            public float IndicatedAirspeedKts { get; set; }
            public float MachNumber { get; set; }
            public float VerticalSpeed { get; set; }
            public float GroundSpeedKts { get; set; }
            public float Pitch { get; set; }
            public float Bank { get; set; }
            public float HeadingMagnetic { get; set; }
            public float HeadingTrue { get; set; }
            public float CourseTrue { get; set; }
            public float GForce { get; set; }
            public float SideForce { get; set; }
            //public GearState GearState { get; set; }
            //   public float GroundSpeed { get; set; }
            // public float IndicatedAirspeed { get; set; }
            // public bool IsAutopilotOn { get; set; }
            //public bool IsCrashed { get; set; }
            //public bool IsLanded { get; set; }
            //public bool IsOnGround { get; set; }
            //public bool IsOverLandingWeight { get; set; }
            //public bool IsOverTakeoffWeight { get; set; }
            //public bool IsPushbackActive { get; set; }
            //public Coordinate Location { get; set; }
            // public float MagneticDeviation { get; set; }
            //public bool ReverseThrustState { get; set; }
            //public SpoilersPosition SpoilersPosition { get; set; }
            //public bool Stalling { get; set; }
            //public float StallProximity { get; set; }
            //public bool StallWarning { get; set; }
            //public float TrueAirspeed { get; set; }
            //public float Velocity { get; set; }
            //public float Weight { get; set; }
            //public float WeightPercentage { get; set; }
        }

        private AircraftStatusForDisplay pAcStateDisplay;
        private Fds.IFAPI.APIAircraftState pAcState;
        public Fds.IFAPI.APIAircraftState AircraftState
        {
            get
            {
                if (pAcState == null) { pAcState = new Fds.IFAPI.APIAircraftState(); }
                if(pAcStateDisplay == null) { pAcStateDisplay = new AircraftStatusForDisplay(); }
                return pAcState;
            }
            set
            {
                pAcState = value;
                if(pAcStateDisplay == null) { pAcStateDisplay = new AircraftStatusForDisplay(); }
                pAcStateDisplay.AltitudeAGL = pAcState.AltitudeAGL;
                pAcStateDisplay.AltitudeMSL = pAcState.AltitudeMSL;
                pAcStateDisplay.Bank = pAcState.Bank;
                pAcStateDisplay.CourseTrue = pAcState.CourseTrue;
                pAcStateDisplay.GForce = pAcState.GForce;
                pAcStateDisplay.GroundSpeedKts = pAcState.GroundSpeedKts;
                pAcStateDisplay.HeadingMagnetic = pAcState.HeadingMagnetic;
                pAcStateDisplay.HeadingTrue = pAcState.HeadingTrue;
                pAcStateDisplay.IndicatedAirspeedKts = pAcState.IndicatedAirspeedKts;
                pAcStateDisplay.MachNumber = pAcState.MachNumber;
                pAcStateDisplay.Pitch = pAcState.Pitch;
                pAcStateDisplay.SideForce = pAcState.SideForce;
                pAcStateDisplay.VerticalSpeed = pAcState.VerticalSpeed;
                
                updateView();
            }
        }

        private void updateView()
        {
            Dictionary<String, object> acStateDict = new Dictionary<String, object>();
            var props = typeof(AircraftStatusForDisplay).GetProperties();
            foreach (var prop in props)
            {
                object value = prop.GetValue(pAcStateDisplay, null); // against prop.Name
                if (prop.Name != "Type")
                {
                    acStateDict.Add(prop.Name, value);
                }
            }
            listView.ItemsSource = acStateDict;

            //AutoPilot Light
            if(pAcState.IsAutopilotOn)
            { ApOff.Visibility = Visibility.Collapsed;  }
            else { ApOff.Visibility = Visibility.Visible; }

            //Gear Lights
            if (pAcState.GearState == Fds.IFAPI.GearState.Down)
            {
                GearDownOff.Visibility = Visibility.Collapsed;
                GearUpOff.Visibility = Visibility.Visible;
            }
            else
            {
                GearDownOff.Visibility = Visibility.Visible;
                GearUpOff.Visibility = Visibility.Collapsed;
            }

            if (pAcState.StallWarning)
            {
                StallWarnOff.Visibility = Visibility.Collapsed;
            }
            else
            {
                StallWarnOff.Visibility = Visibility.Visible;
            }

            if (pAcState.Stalling)
            {
                StallOff.Visibility = Visibility.Collapsed;
            }
            else
            {
                StallOff.Visibility = Visibility.Visible;
            }
        }
    }
}
