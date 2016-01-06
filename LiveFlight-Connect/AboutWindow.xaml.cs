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
using System.Windows.Shapes;

namespace LiveFlight
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            
        }


        private void cameronButton_Click(object sender, RoutedEventArgs e)
        {
            var URL = "http://www.carmichaelalonso.co.uk";
            System.Diagnostics.Process.Start(URL);
        }

        private void arButton_Click(object sender, RoutedEventArgs e)
        {
            var URL = "https://community.infinite-flight.com/users/ar_ar";
            System.Diagnostics.Process.Start(URL);
        }

        private void licenseButton_Click(object sender, RoutedEventArgs e)
        {
            var URL = "https://github.com/LiveFlightApp/Connect-Windows/blob/master/LICENSE";
            System.Diagnostics.Process.Start(URL);
        }

        private void sourceCode_Click(object sender, RoutedEventArgs e)
        {
            var URL = "https://github.com/LiveFlightApp/Connect-Windows/";
            System.Diagnostics.Process.Start(URL);
        }

        private void liveFlightLink_Click(object sender, RoutedEventArgs e)
        {
            var URL = "http://www.liveflightapp.com";
            System.Diagnostics.Process.Start(URL);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            version.Content = String.Format("Version {0:0.0}", Versioning.currentAppVersion);
        }
    }
}
