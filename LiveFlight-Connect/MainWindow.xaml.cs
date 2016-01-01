///
/// This is a sample project for the Infinite Flight API
/// 

using Fds.IFAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using System.Windows.Forms;

namespace LiveFlight
{

    public partial class MainWindow : Window
    {
        public static IFConnectorClient client = new IFConnectorClient();
        BroadcastReceiver receiver = new BroadcastReceiver();
        Commands commands = new Commands();

        public MainWindow()
        {
            InitializeComponent();

            airplaneStateGrid.DataContext = null;
            airplaneStateGrid.DataContext = new APIAircraftState();

            mainTabControl.Visibility = System.Windows.Visibility.Collapsed;

            Console.WriteLine("LiveFlight Connect\nPlease send this log to cameron@liveflightapp.com if you experience issues. Thanks!\n\n");
        }

        bool serverInfoReceived = false;

        void receiver_DataReceived(object sender, EventArgs e)
        {
            byte[] data = (byte[])sender;

            var apiServerInfo = Serializer.DeserializeJson<APIServerInfo>(UTF8Encoding.UTF8.GetString(data));

            if (apiServerInfo != null)
            {
                Console.WriteLine("Received Server Info from: {0}:{1}", apiServerInfo.Address, apiServerInfo.Port);
                serverInfoReceived = true;
                receiver.Stop();
                Dispatcher.BeginInvoke((Action)(() => 
                {
                    Connect(IPAddress.Parse(apiServerInfo.Address), apiServerInfo.Port);
                }));
            }
            else
            {
                Console.WriteLine("Invalid Server Info Received");
            }
        }

        private void Connect(IPAddress iPAddress, int port)
        {
            client.Connect(iPAddress.ToString(), port);

            // set label text
            ipLabel.Content = String.Format("Infinite Flight is at {0}", iPAddress.ToString());
           
            overlayGrid.Visibility = System.Windows.Visibility.Collapsed;
            mainTabControl.Visibility = System.Windows.Visibility.Visible;

            client.CommandReceived += client_CommandReceived;

            client.SendCommand(new APICall { Command = "InfiniteFlight.GetStatus" });
            client.SendCommand(new APICall { Command = "Live.EnableATCMessageListUpdated" });            

            Task.Run(() =>
            {

                while (true)
                {
                    try
                    {
                        client.SendCommand(new APICall { Command = "Airplane.GetState" });

                        Thread.Sleep(2000);

                    }
                    catch (Exception ex)
                    {

                    }
                }
            });

            Task.Run(() =>
            {

                while (true)
                {
                    try
                    {
                        client.SendCommand(new APICall { Command = "Live.GetTraffic" });
                        client.SendCommand(new APICall { Command = "Live.ATCFacilities" });

                        Thread.Sleep(5000);

                    }
                    catch (Exception ex)
                    {

                    }
                }
            });
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            receiver.DataReceived += receiver_DataReceived;
            receiver.StartListening();
        }

        void client_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => 
            {
                var type = Type.GetType(e.Response.Type);

                if (type == typeof(APIAircraftState))
                {
                    var state = Serializer.DeserializeJson<APIAircraftState>(e.CommandString);

                    airplaneStateGrid.DataContext = null;
                    airplaneStateGrid.DataContext = state;
                }
                else if (type == typeof(GetValueResponse))
                {
                    var state = Serializer.DeserializeJson<GetValueResponse>(e.CommandString);

                    Console.WriteLine("{0} -> {1}", state.Parameters[0].Name, state.Parameters[0].Value);
                }
                else if (type == typeof(LiveAirplaneList))
                {
                    var airplaneList = Serializer.DeserializeJson<LiveAirplaneList>(e.CommandString);

                    //airplaneDataGrid.ItemsSource = airplaneList.Airplanes;
                }
                else if (type == typeof(FacilityList))
                {
                    var facilityList = Serializer.DeserializeJson<FacilityList>(e.CommandString);

                    //facilitiesDataGrid.ItemsSource = facilityList.Facilities;
                }
                else if (type == typeof(IFAPIStatus))
                {
                    var status = Serializer.DeserializeJson<IFAPIStatus>(e.CommandString);


                }
                else if (type == typeof(APIATCMessage))
                {
                    var msg = Serializer.DeserializeJson<APIATCMessage>(e.CommandString);
                    // TODO client.ExecuteCommand("Live.GetCurrentCOMFrequencies");
                }
                else if (type == typeof(APIFrequencyInfoList))
                {
                    var msg = Serializer.DeserializeJson<APIFrequencyInfoList>(e.CommandString);
                }
                else if (type == typeof(ATCMessageList))
                {
                    var msg = Serializer.DeserializeJson<ATCMessageList>(e.CommandString);
                    atcMessagesDataGrid.ItemsSource = msg.ATCMessages;
                }
                else if (type == typeof(APIFlightPlan))
                {
                    var msg = Serializer.DeserializeJson<APIFlightPlan>(e.CommandString);
                    Console.WriteLine("Flight Plan: {0} items", msg.Waypoints.Length);

                    foreach (var item in msg.Waypoints)
                    {
                        Console.WriteLine(" -> {0} {1} - {2}, {3}", item.Name, item.Code, item.Latitude, item.Longitude);
                    }
                }             
            }));            
        }

        private void toggleBrakesButton_Click(object sender, RoutedEventArgs e)
        {
            //client.SetValue("Aircraft.Systems.Autopilot.EnableHeading", "True");
        }
        
        private void toggleBrakesButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            client.ExecuteCommand("Commands.Brakes", new CallParameter[] { new CallParameter { Name = "KeyAction", Value = "Down" } } );
        }

        private void toggleBrakesButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            client.ExecuteCommand("Commands.Brakes", new CallParameter[] { new CallParameter { Name = "KeyAction", Value = "Up" } });
        }

        private void parkingBrakeButton_Click(object sender, RoutedEventArgs e)
        {
            client.ExecuteCommand("Commands.ParkingBrakes");
            client.GetValue("Aircraft.State.IsBraking");
        }

        private void prevCameraButton_Click(object sender, RoutedEventArgs e)
        {
            client.ExecuteCommand("Commands.PrevCamera");
        }

        private void nextCameraButton_Click(object sender, RoutedEventArgs e)
        {
            client.ExecuteCommand("Commands.NextCamera");
        }


        private void setGearStateButton_Click(object sender, RoutedEventArgs e)
        {
            client.ExecuteCommand("Commands.LandingGear");
        }


        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;

            client.ExecuteCommand("NetworkJoystick.SetButtonState", new CallParameter[] 
            {
                new CallParameter 
                { 
                    Name = button.Content.ToString(),  // button index
                    Value = "Down"
                }
            });
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Button button = sender as System.Windows.Controls.Button;

            client.ExecuteCommand("NetworkJoystick.SetButtonState", new CallParameter[] 
            {
                new CallParameter 
                { 
                    Name = button.Content.ToString(),  // button index
                    Value = "Up"
                }
            });
        }

        private void POVButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var type = button.Content.ToString();

            var xValue = 0;
            var yValue = 0;

            if (type == "Up")
            {
                xValue = 0;
                yValue = 1;
            }
            else if (type == "Down")
            {
                xValue = 0;
                yValue = -1;
            }
            else if (type == "Left")
            {
                xValue = -1;
                yValue = 0;
            }
            else if (type == "Right")
            {
                xValue = 1;
                yValue = 0;
            }

            client.ExecuteCommand("NetworkJoystick.SetPOVState", new CallParameter[] 
                {
                    new CallParameter 
                    { 
                        Name = "X",
                        Value = xValue.ToString()
                    },
                    new CallParameter 
                    { 
                        Name = "Y",
                        Value = yValue.ToString()
                    }
                });


        }

        private void POVButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            var type = button.Content.ToString();

            var xValue = 0;
            var yValue = 0;

            if (type == "Up")
            {
                xValue = 0;
                yValue = 0;
            }
            else if (type == "Down")
            {
                xValue = 0;
                yValue = 0;
            }
            else if (type == "Left")
            {
                xValue = 0;
                yValue = 0;
            }
            else if (type == "Right")
            {
                xValue = 0;
                yValue = 0;
            }

            client.ExecuteCommand("NetworkJoystick.SetPOVState", new CallParameter[] 
                {
                    new CallParameter 
                    { 
                        Name = "X",
                        Value = xValue.ToString()
                    },
                    new CallParameter 
                    { 
                        Name = "Y",
                        Value = yValue.ToString()
                    }
                });
        }
        
        private void checkbox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as System.Windows.Controls.CheckBox;

            if (checkbox.Equals(altitudeStateCheckbox))
                client.ExecuteCommand("Commands.Autopilot.SetAltitudeState", new CallParameter[] { new CallParameter { Value = checkbox.IsChecked.ToString() } });
            if (checkbox.Equals(headingStateCheckbox))
                client.ExecuteCommand("Commands.Autopilot.SetHeadingState", new CallParameter[] { new CallParameter { Value = checkbox.IsChecked.ToString() } });
            if (checkbox.Equals(verticalSpeedStateCheckbox))
                client.ExecuteCommand("Commands.Autopilot.SetVSState", new CallParameter[] { new CallParameter { Value = checkbox.IsChecked.ToString() } });
            if (checkbox.Equals(speedStateCheckbox))
                client.ExecuteCommand("Commands.Autopilot.SetSpeedState", new CallParameter[] { new CallParameter { Value = checkbox.IsChecked.ToString() } });
            if (checkbox.Equals(apprStateCheckbox))
                client.ExecuteCommand("Commands.Autopilot.SetApproachModeState", new CallParameter[] { new CallParameter { Value = checkbox.IsChecked.ToString() } });
        }

        private void speedTextBlock_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBlock = sender as System.Windows.Controls.TextBox;

            if (textBlock.Equals(speedTextBlock))
                client.ExecuteCommand("Commands.Autopilot.SetSpeed", new CallParameter[] { new CallParameter { Value = textBlock.Text.ToString() } });
            if (textBlock.Equals(altitudeTextBlock))
                client.ExecuteCommand("Commands.Autopilot.SetAltitude", new CallParameter[] { new CallParameter { Value = textBlock.Text.ToString() } });
            if (textBlock.Equals(verticalSpeedTextBlock))
                client.ExecuteCommand("Commands.Autopilot.SetVS", new CallParameter[] { new CallParameter { Value = textBlock.Text.ToString() } });
            if (textBlock.Equals(headingTextBlock))
                client.ExecuteCommand("Commands.Autopilot.SetHeading", new CallParameter[] { new CallParameter { Value = textBlock.Text.ToString() } });

        }

        private void enableATCMessagesButton_Click(object sender, RoutedEventArgs e)
        {
            client.ExecuteCommand("Live.EnableATCMessageNotification");
        }


        private void atcMessagesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var command = string.Format("Commands.ATCEntry{0}", atcMessagesDataGrid.SelectedIndex + 1);

            client.ExecuteCommand(command);            
        }

        private void setCameraPosition_Click(object sender, RoutedEventArgs e)
        {
            var command = "Cameras.SetATCCameraPosition";
            
            client.ExecuteCommand(command, new CallParameter[]
            {
                new CallParameter { Name = "Latitude", Value = "33.950681" }, 
                new CallParameter { Name = "Longitude", Value = "-118.401479" },
                new CallParameter { Name = "Altitude", Value = "-110" }
            });    
        }

        Point lastMousePosition = new Point();

        private void captureMouseButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void captureMouseButton_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            var position = e.GetPosition(this);

            var command = "NetworkMouse.SetPosition";
            
            client.ExecuteCommand(command, new CallParameter[]
            {
                new CallParameter { Name = "X", Value = ((int)position.X).ToString() }, 
                new CallParameter { Name = "Y", Value = ((int)position.Y).ToString() }
            });

            lastMousePosition = position;
        }

        private void captureMouseButton_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var position = lastMousePosition;

            var command = "NetworkMouse.MouseUp";

            client.ExecuteCommand(command, new CallParameter[]
            {
                new CallParameter { Name = "X", Value = ((int)position.X).ToString() }, 
                new CallParameter { Name = "Y", Value = ((int)position.Y).ToString() }
            });   
        }

        private void captureMouseButton_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var position = lastMousePosition;

            var command = "NetworkMouse.MouseDown";

            client.ExecuteCommand(command, new CallParameter[]
            {
                new CallParameter { Name = "X", Value = ((int)position.X).ToString() }, 
                new CallParameter { Name = "Y", Value = ((int)position.Y).ToString() }
            });   
        }

        private void mainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        protected bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Console.WriteLine("Key pressed: {0}", keyData.ToString());

            //ATC
            if (keyData == Keys.D1)
            {
                commands.atc1();

            }
            else if (keyData == Keys.D2)
            {
                commands.atc2();

            }
            else if (keyData == Keys.D3)
            {
                commands.atc3();

            }
            else if (keyData == Keys.D4)
            {
                commands.atc4();

            }
            else if (keyData == Keys.D5)
            {
                commands.atc5();

            }
            else if (keyData == Keys.D6)
            {
                commands.atc6();

            }
            else if (keyData == Keys.D7)
            {
                commands.atc7();

            }
            else if (keyData == Keys.D8)
            {
                commands.atc8();

            }
            else if (keyData == Keys.D9)
            {
                commands.atc9();

            }
            else if (keyData == Keys.D0)
            {
                commands.atc10();

            }
            else if (keyData == Keys.A)
            {
                //toggle atc window
                commands.atcMenu();
            }

            //flight controls
            if (keyData == Keys.G)
            {
                //toggle landing gear
                commands.landingGear();
            }
            else if (keyData == (Keys.P | Keys.Shift))
            {
                //shift + p
                //pushback
                commands.pushback();
            }
            else if (keyData == Keys.P)
            {
                //pause
                commands.pause();

            }
            else if (keyData == Keys.OemPeriod)
            {
                //brake
                commands.parkingBrake();
            }
            else if (keyData == Keys.F6)
            {
                //retract flaps
                commands.flapsUp();
            }

            else if (keyData == Keys.F7)
            {
                //extend flaps
                commands.flapsDown();
            }
            else if (keyData == Keys.OemQuestion)
            {
                //spoilers
                commands.spoilers();
            }
            else if (keyData == Keys.L)
            {
                //kanding lights
                commands.landing();
            }
            else if (keyData == Keys.S)
            {
                //strobe
                commands.strobe();
            }
            else if (keyData == Keys.N)
            {
                //nav
                commands.nav();
            }
            else if (keyData == Keys.B)
            {
                //beacon
                commands.beacon();
            }
            else if (keyData == Keys.Z)
            {
                //toggle autopilot
                commands.autopilot();
            }
            else if (keyData == Keys.OemMinus)
            {
                //zoomout
                commands.zoomOut();
            }
            else if (keyData == Keys.Oemplus)
            {
                //zoomout
                commands.zoomIn();
            }
            /*else if (keyData == Keys.Up)
            {
                //move up
                commands.movePOV(0);
                Thread.Sleep(2000);
                
            }
            else if (keyData == Keys.Down)
            {
                //move down
                commands.movePOV(18000);
                Thread.Sleep(2000);

            }
            else if (keyData == Keys.Left)
            {
                //move left
                commands.movePOV(27000);
                Thread.Sleep(2000);

            }
            else if (keyData == Keys.Right)
            {
                //move right
                commands.movePOV(9000);
                Thread.Sleep(2000);

            }*/
            else if (keyData == Keys.D)
            {
                //Next Camera
                commands.nextCamera();
            }
            else if (keyData == (Keys.D | Keys.Shift))
            {
                //previous Camera
                commands.previousCamera();
            }


            return true;
        }

    }
}
