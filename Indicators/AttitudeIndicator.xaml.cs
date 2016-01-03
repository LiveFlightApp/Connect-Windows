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

namespace Indicators
{
    /// <summary>
    /// Interaction logic for IndicatorControl.xaml
    /// </summary>
    public partial class AttitudeIndicator : UserControl
    {
        public void updateAttitude(double pitch, double roll)
        {
            attitude.pitch = pitch;
            attitude.roll = -1*roll;
            applyRotTranslation();
        }

        public AttitudeIndicator()
        {
            InitializeComponent();
            attitude = new pitchAndRoll();
        }

        private struct pitchAndRoll
        {
            public double pitch;
            public double roll;
        }

        private pitchAndRoll attitude;


        private void applyRotTranslation()
        {


           // UIElement container = VisualTreeHelper.GetParent(centerCircle) as UIElement;
           // Point center = centerCircle.TranslatePoint(new Point(0, 0), container);

            TransformGroup tg = new TransformGroup();

            //The attitude scale is 5.5px/deg until +/-30deg where it changes to ~2.75px/deg
            double translation = 0.0;
            if (attitude.pitch > 30)
            {
                translation = (5.5 * 30) + ((attitude.pitch - 30) * 2.75);
            }
            else if (attitude.pitch < -30)
            {
                translation = (5.5 * -30) + ((attitude.pitch + 30) * 2.75);
            }
            else //+/-30 deg
            {
                translation = attitude.pitch * 5.5;
            }

            TranslateTransform t = new TranslateTransform(0, translation);
            RotateTransform r = new RotateTransform(attitude.roll); //, center.X, center.Y); // (horizon.X2 - horizon.X1) / 2, (horizon.Y2 - horizon.Y1) / 2);
            //if (attitude.pitch < 0 && attitude.roll < 0)
            //{
            //    tg.Children.Add(r);
            //    tg.Children.Add(t);
            //}
            //else
            //{
                tg.Children.Add(t);
                tg.Children.Add(r);
            // }
            PitchScaleLines.RenderTransform = tg;
            ArtHorizon.RenderTransform = tg;
            scaleCanvas.RenderTransform = tg;
            BankRing.RenderTransform = r;

        }
    }
}
