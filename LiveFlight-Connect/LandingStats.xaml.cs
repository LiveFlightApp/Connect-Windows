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
using Fds.IFAPI;

namespace LiveFlight
{
    /// <summary>
    /// Interaction logic for LandingStats.xaml
    /// </summary>
    public partial class LandingStats : UserControl
    {
        public LandingStats()
        {
            InitializeComponent();
        }


        public class touchdownInfo
        {
            public string aircraftType { get; set; }
            public double rollDistance { get; set; }
            public double touchdownGroundSpeed { get; set; }
            public double touchdownIAS { get; set; }
            public double touchdownGforce { get; set; }
            public double touchdownVS { get; set; }
            public double touchdownPitch { get; set; }
            public bool wasStalling { get; set; }
            public bool wasNearStall { get; set; }
            public bool wasOverMLW { get; set; }
        }

        private touchdownInfo pTouchdownInfo;

        public void updateLandingStats(Coordinate touchdownPosition, Coordinate positionAtEndOfRoll, APIAircraftState stateJustBeforeTouchdown, APIAircraftState stateJustAfterTouchdown, string aircraftType)
        {
            pTouchdownInfo = new touchdownInfo();
           // pTouchdownInfo.aircraftType = aircraftType;
            pTouchdownInfo.touchdownGroundSpeed = stateJustBeforeTouchdown.GroundSpeedKts;
            pTouchdownInfo.touchdownIAS = stateJustBeforeTouchdown.IndicatedAirspeedKts;
            pTouchdownInfo.touchdownPitch = stateJustBeforeTouchdown.Pitch;
            pTouchdownInfo.touchdownGforce = stateJustBeforeTouchdown.GForce > stateJustAfterTouchdown.GForce ? stateJustBeforeTouchdown.GForce : stateJustAfterTouchdown.GForce;
            pTouchdownInfo.touchdownVS = stateJustBeforeTouchdown.VerticalSpeed;
            pTouchdownInfo.wasStalling = stateJustBeforeTouchdown.Stalling;
            pTouchdownInfo.wasNearStall = stateJustBeforeTouchdown.StallWarning;
            pTouchdownInfo.wasOverMLW = stateJustBeforeTouchdown.IsOverLandingWeight;

            pTouchdownInfo.rollDistance = calcRollDistance(touchdownPosition,positionAtEndOfRoll);


            gridLandingStats.DataContext = pTouchdownInfo;

        }

        private double calcRollDistance(Coordinate touchdownPosition, Coordinate currentPosition)
        {
            var R = 3959; // Radius of the earth in miles
            var dLat = (currentPosition.Latitude - touchdownPosition.Latitude) * (Math.PI / 180);
            var dLon = (currentPosition.Longitude - touchdownPosition.Longitude) * (Math.PI / 180);
            var a =
              Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
              Math.Cos((currentPosition.Latitude) * (Math.PI / 180)) * Math.Cos((touchdownPosition.Latitude) * (Math.PI / 180)) *
              Math.Sin(dLon / 2) * Math.Sin(dLon / 2)
              ;
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c; // Distance in miles
            d *= 5280; //Distance in ft
            return d;
        }

    }
}
