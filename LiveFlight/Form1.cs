using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fds.IFAPI;
using SharpDX.DirectInput;
using System.Windows.Input;

namespace LiveFlight
{
    public partial class LiveFlight : Form
    {

        public static IFConnectorClient client = new IFConnectorClient();
        BroadcastReceiver receiver = new BroadcastReceiver();
        JoystickMap mapping = new JoystickMap();
        Commands commands = new Commands();
        Joystick joystick;

        String ipAddressString = "";
        bool isApOn;
        int heading;

        public LiveFlight()
        {
            InitializeComponent();
        }

        private async void LiveFlight_Load(object sender, EventArgs e)
        {

            panel1.BringToFront();
            label1.Text = "Connecting...\n\nMake sure this PC is on the same network as the device with Infinite Flight running and Infinite Flight Connect enabled.";
            label1.BackColor = Color.Transparent;

            //check version - important
            bool x = await checkVersion();

            //start UDP receiver
            receiver.DataReceived += receiver_DataReceived;
            receiver.StartListening();

            await Task.Run(() =>
            {
                beginJoystickPoll();

            });

            setMappingLabels();

            degreesTrackBar.ValueChanged += new System.EventHandler(degreesChanged);


        }

        /*private static async Task<bool> checkVersion()
        {
            int currentVersion = 1;

            ParseConfig config = null;
            try
            {
                config = await ParseConfig.GetAsync();
            }
            catch (Exception e)
            {
                config = ParseConfig.CurrentConfig;
            }

            int windowsBuild = 0;
            bool result = config.TryGetValue("WindowsBuild", out windowsBuild);

            Console.WriteLine(String.Format("Local Build = {0}", currentVersion));
            Console.WriteLine(String.Format("Server Build = {0}", windowsBuild));

            if (windowsBuild > currentVersion)
            {
                Console.WriteLine("An update is available!");

            }

            Console.WriteLine("\n\n");

            return true;
        }*/

        void setMappingLabels()
        {
            mapping.open();

            brakesButtonLabel.Text = mapping.BrakesButtonId;
            flapUpLabel.Text = mapping.FlapUpId;
            flapDownLabel.Text = mapping.FlapDownId;
            previousCameraLabel.Text = mapping.PreviousButtonId;
            nextCameraLabel.Text = mapping.NextButtonId;
            gearLabel.Text = mapping.GearId;
            spoilersLabel.Text = mapping.SpoilersId;
            reverseThrustLabel.Text = mapping.ReverseId;
            autopilotLabel.Text = mapping.AutopilotId;
            assignHUDLabel.Text = mapping.HudId;
            parkingBrakeLabel.Text = mapping.ParkingBrakeId;
            pushbackLabel.Text = mapping.PushbackId;
            assignPauseLabel.Text = mapping.PauseId;
            assignTrimUpLabel.Text = mapping.TrimUpId;
            assignTrimDownLabel.Text = mapping.TrimDownId;
            assignATCToggleLabel.Text = mapping.AtcMenuId;
            assignLandingLabel.Text = mapping.LandingId;
            assignNavLabel.Text = mapping.NavId;
            assignStrobeLabel.Text = mapping.StrobeId;
            assignBeaconLabel.Text = mapping.BeaconId;

        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
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

        void receiver_DataReceived(object sender, EventArgs e)
        {
            byte[] data = (byte[])sender;

            var apiServerInfo = Serializer.DeserializeJson<APIServerInfo>(UTF8Encoding.UTF8.GetString(data));

            if (apiServerInfo != null)
            {
                Console.WriteLine("Received Server Info from: {0}:{1}", apiServerInfo.Address, apiServerInfo.Port);
                receiver.Stop();
                ipAddressString = apiServerInfo.Address;

                this.Invoke((MethodInvoker)delegate {
                    //statusLabel.Text = "Infinite Flight found on IP: " + ipAddressString + " - tap above to connect.";
                    button1.Enabled = true;
                    connectToInfiniteFlight();
                    panel1.Visible = false;
                });

                //foundIP();
            }
            else
            {
                Console.WriteLine("Invalid Server Info Received");
            }
        }


        public void connectToInfiniteFlight()
        {

            Task.Run(() =>
            {
                Console.WriteLine("Connecting to host...");
                var host = ipAddressString;

                if (client.Connect(host) == true)
                {
                    //successfully connected

                    this.Invoke((MethodInvoker)delegate
                    {
                        button1.Enabled = false;
                        button1.Text = "Connected!";
                        statusLabel.Text = "Connected to Infinite Flight on IP: " + ipAddressString;
                    });

                    client.CommandReceived += client_CommandReceived;
                    client.SendCommand(new APICall { Command = "InfiniteFlight.GetStatus" });

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

                }
                else
                {

                    this.Invoke((MethodInvoker)delegate
                    {
                        button1.Enabled = true;
                        button1.Text = "Connect to Infinite Flight";
                        statusLabel.Text = "There was a problem connecting. Please try again.";
                    });

                }

            });
        }

        void client_CommandReceived(object sender, CommandReceivedEventArgs e)
        {
            //Dispatcher.BeginInvoke((Action)(() =>
            //{
                var type = Type.GetType(e.Response.Type);

                if (type == typeof(APIAircraftState))
                {
                    var state = Serializer.DeserializeJson<APIAircraftState>(e.CommandString);
                    isApOn = state.IsAutopilotOn;
                    degreesTrackBar.Value = Convert.ToInt32(state.HeadingTrue);
                }
                else if (type == typeof(GetValueResponse))
                {
                    var state = Serializer.DeserializeJson<GetValueResponse>(e.CommandString);

                    Console.WriteLine("{0} -> {1}", state.Parameters[0].Name, state.Parameters[0].Value);
                }
                else if (type == typeof(LiveAirplaneList))
                {
                    var airplaneList = Serializer.DeserializeJson<LiveAirplaneList>(e.CommandString);

                }
                else if (type == typeof(FacilityList))
                {
                    var facilityList = Serializer.DeserializeJson<FacilityList>(e.CommandString);

                }
                else if (type == typeof(IFAPIStatus))
                {
                    var status = Serializer.DeserializeJson<IFAPIStatus>(e.CommandString);

                this.Invoke((MethodInvoker)delegate
                {
                    appVersion.Text = status.AppVersion;
                    loggedInUser.Text = status.LoggedInUser;

                    Console.WriteLine("API Version: {0}", status.ApiVersion);

                });

                }
            //}));

        }


        private void beginJoystickPoll()
        {


            // Initialize DirectInput
            var directInput = new DirectInput();

            // Find a Joystick Guid
            var joystickGuid = Guid.Empty;

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad,
                        DeviceEnumerationFlags.AllDevices))
                joystickGuid = deviceInstance.InstanceGuid;

            // If Gamepad not found, look for a Joystick
            if (joystickGuid == Guid.Empty)
                foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,
                        DeviceEnumerationFlags.AllDevices))
                    joystickGuid = deviceInstance.InstanceGuid;

            // If Joystick not found, throws an error
            if (joystickGuid == Guid.Empty)
            {
                Console.WriteLine("No joystick found.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            // Instantiate the joystick
            joystick = new Joystick(directInput, joystickGuid);

            Console.WriteLine("Found Joystick with GUID: {0}", joystickGuid);
            Console.WriteLine("Joystick - {0}", joystick.Properties.ProductName);

            // Query all suported ForceFeedback effects
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
                Console.WriteLine("Effect available {0}", effectInfo.Name);

            // Set BufferSize in order to use buffered data.
            joystick.Properties.BufferSize = 128;

            // Acquire the joystick
            joystick.Acquire();

            // Poll events from joystick
            while (true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine(state);
                    Console.WriteLine(state.Offset);

                    if (state.Offset.ToString().StartsWith("Button") || state.Offset.ToString().StartsWith("Point"))
                    {

                        buttonPressed(state.Offset, state.Value);

                    }
                    else
                    {

                        if (state.Offset.ToString() != "RotationZ" && state.Offset.ToString() != "Z")
                        {
                            //isn't rudder nor throttle

                            if (axisMoveAutopilotDisable.Checked == true)
                            {
                                if (isApOn == true)
                                {
                                    Console.WriteLine("Disable AP");
                                    commands.autopilot();
                                    isApOn = false;
                                }

                            }

                        }

                        //is an axis
                        axisMoved(state.Offset, state.Value, 32767);

                    }

                }
            }

        }

        private void buttonPressed(JoystickOffset offset, int value)
        {

            //is a button
            if (offset.ToString() == mapping.BrakesButtonId)
            {
                //call brakes
                if (value == 0)
                {
                    client.ExecuteCommand("Commands.Brakes", new CallParameter[] { new CallParameter { Name = "KeyAction", Value = "Up" } });

                }
                else
                {
                    client.ExecuteCommand("Commands.Brakes", new CallParameter[] { new CallParameter { Name = "KeyAction", Value = "Down" } });
                }

            }
            else if (offset.ToString() == mapping.PreviousButtonId)
            {
                if (value != 0)
                {
                    commands.previousCamera();
                }

            }
            else if (offset.ToString() == mapping.NextButtonId)
            {
                if (value != 0)
                {
                    commands.nextCamera();
                }
            }
            else if (offset.ToString() == mapping.FlapDownId)
            {
                if (value != 0)
                {
                    commands.flapsDown();
                }
            }
            else if (offset.ToString() == mapping.FlapUpId)
            {
                if (value != 0)
                {
                    commands.flapsUp();
                }
            }
            else if (offset.ToString() == mapping.GearId)
            {
                if (value != 0)
                {
                    commands.landingGear();
                }
            }
            else if (offset.ToString() == mapping.SpoilersId)
            {
                if (value != 0)
                {
                    commands.spoilers();
                }
            }
            else if (offset.ToString() == "PointOfViewControllers0")
            {
                commands.movePOV(value);

            }
            else if (offset.ToString() == mapping.ReverseId)
            {
                if (value != 0)
                {
                    commands.reverseThrust();
                }
            }
            else if (offset.ToString() == mapping.AutopilotId)
            {
                if (value != 0)
                {
                    commands.autopilot();
                }
            }
            else if (offset.ToString() == mapping.HudId)
            {
                if (value != 0)
                {
                    commands.hud();
                }
            }
            else if (offset.ToString() == mapping.ParkingBrakeId)
            {
                if (value != 0)
                {
                    commands.parkingBrake();
                }
            }
            else if (offset.ToString() == mapping.PushbackId)
            {
                if (value != 0)
                {
                    commands.pushback();
                }
            }
            else if (offset.ToString() == mapping.PauseId)
            {
                if (value != 0)
                {
                    commands.pause();
                }
            }
            else if (offset.ToString() == mapping.TrimUpId)
            {
                if (value != 0)
                {
                    commands.trimUp();
                }
            }
            else if (offset.ToString() == mapping.TrimDownId)
            {
                if (value != 0)
                {
                    commands.trimDown();
                }
            }
            else if (offset.ToString() == mapping.AtcMenuId)
            {
                if (value != 0)
                {
                    commands.atcMenu();
                }
            }
            else if (offset.ToString() == mapping.LandingId)
            {
                if (value != 0)
                {
                    commands.landing();
                }
            }
            else if (offset.ToString() == mapping.NavId)
            {
                if (value != 0)
                {
                    commands.nav();
                }
            }
            else if (offset.ToString() == mapping.StrobeId)
            {
                if (value != 0)
                {
                    commands.strobe();
                }
            }
            else if (offset.ToString() == mapping.BeaconId)
            {
                if (value != 0)
                {
                    commands.beacon();
                }
            }

        }

        private void axisMoved(JoystickOffset offset, int value, int range)
        {

            //indexes
            // 0 - pitch
            // 1 - roll

            var indexOf = -1;

            if (offset.ToString() == "X")
            {

                indexOf = 1;

            }
            else if (offset.ToString() == "Y")
            {

                indexOf = 0;

            }
            else if (offset.ToString() == "RotationZ")
            {

                indexOf = 2;

            }
            else if (offset.ToString() == "Z")
            {

                indexOf = 3;

            }

            if (indexOf < 2)
            {
                //divide value by 10
                value = value - range;
                value = value / 32;

            } else
            {

                if (indexOf == 3)
                {
                    if (reverseThrottle.CheckState == CheckState.Checked)
                    {
                        //reverse throttle

                        value = value - range;
                        value = value * -1;
                        value = value / 32;

                    } else
                    {
                        value = value - range;
                        value = value / 32;
                    }


                }
                else
                {
                    //is rudder

                    value = value - range;
                    value = value / 32;

                }

            }


            if (indexOf != -1)
            {

                client.ExecuteCommand("NetworkJoystick.SetAxisValue", new CallParameter[]
                {
                new CallParameter
                {
                    Name = indexOf.ToString(),
                    Value = value.ToString()
                }
                });

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            connectToInfiniteFlight();
            

        }

        public void previousCamera(object sender, EventArgs e)
        {
            commands.previousCamera();
        }

        public void nextCamera(object sender, EventArgs e)
        {
            commands.nextCamera();
        }


        private void brakesButton_Click(object sender, EventArgs e)
        {
            brakesButtonLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Brakes: " + state.Offset);
                    mapping.BrakesButtonId = state.Offset.ToString();
                    brakesButtonLabel.Text = mapping.BrakesButtonId;
                    run = false;

                }
            }

        }


        private void assignFlapsUp_Click(object sender, EventArgs e)
        {
            flapUpLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Flaps Up: " + state.Offset);
                    mapping.FlapUpId = state.Offset.ToString();
                    flapUpLabel.Text = mapping.FlapUpId;
                    run = false;

                }
            }
        }

        private void assignFlapsDown_Click(object sender, EventArgs e)
        {
            flapDownLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Flaps Down: " + state.Offset);
                    mapping.FlapDownId = state.Offset.ToString();
                    flapDownLabel.Text = mapping.FlapDownId;
                    run = false;

                }
            }
        }

        private void assignGear_Click(object sender, EventArgs e)
        {
            gearLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Gear: " + state.Offset);
                    mapping.GearId = state.Offset.ToString();
                    gearLabel.Text = mapping.GearId;
                    run = false;

                }
            }
        }

        private void assignSpoilers_Click(object sender, EventArgs e)
        {
            spoilersLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Spoilers: " + state.Offset);
                    mapping.SpoilersId = state.Offset.ToString();
                    spoilersLabel.Text = mapping.SpoilersId;
                    run = false;

                }
            }
        }

        private void assignPreviousCamera_Click(object sender, EventArgs e)
        {
            previousCameraLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Previous Camera: " + state.Offset);
                    mapping.PreviousButtonId = state.Offset.ToString();
                    previousCameraLabel.Text = mapping.PreviousButtonId;
                    run = false;

                }
            }

        }

        private void assignNextCamera_Click(object sender, EventArgs e)
        {
            nextCameraLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Next Camera: " + state.Offset);
                    mapping.NextButtonId = state.Offset.ToString();
                    nextCameraLabel.Text = mapping.NextButtonId;
                    run = false;

                }
            }

        }

        private void assignReverseThrust_Click(object sender, EventArgs e)
        {
            reverseThrustLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Reverse Thrust: " + state.Offset);
                    mapping.ReverseId = state.Offset.ToString();
                    reverseThrustLabel.Text = mapping.ReverseId;
                    run = false;

                }
            }
        }

        private void assignAutopilot_Click(object sender, EventArgs e)
        {
            autopilotLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Autopilot: " + state.Offset);
                    mapping.AutopilotId = state.Offset.ToString();
                    autopilotLabel.Text = mapping.AutopilotId;
                    run = false;

                }
            }

        }

        private void assignHUD_Click(object sender, EventArgs e)
        {
            assignHUDLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Toggle HUD: " + state.Offset);
                    mapping.HudId = state.Offset.ToString();
                    assignHUDLabel.Text = mapping.HudId;
                    run = false;

                }
            }

        }

        private void assignParkingBrake_Click(object sender, EventArgs e)
        {
            parkingBrakeLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Parking Brake: " + state.Offset);
                    mapping.ParkingBrakeId = state.Offset.ToString();
                    parkingBrakeLabel.Text = mapping.ParkingBrakeId;
                    run = false;

                }
            }

        }

        private void assignPushback_Click(object sender, EventArgs e)
        {
            pushbackLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Pushback: " + state.Offset);
                    mapping.PushbackId = state.Offset.ToString();
                    pushbackLabel.Text = mapping.PushbackId;
                    run = false;

                }
            }

        }

        private void assignPause_Click(object sender, EventArgs e)
        {
            assignPauseLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Pause: " + state.Offset);
                    mapping.PauseId = state.Offset.ToString();
                    assignPauseLabel.Text = mapping.PauseId;
                    run = false;

                }
            }
        }

        private void assignTrimUp_Click(object sender, EventArgs e)
        {
            assignTrimUpLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Trim up: " + state.Offset);
                    mapping.TrimUpId = state.Offset.ToString();
                    assignTrimUpLabel.Text = mapping.TrimUpId;
                    run = false;

                }
            }

        }

        private void assignTrimDown_Click(object sender, EventArgs e)
        {
            assignTrimDownLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Trim down: " + state.Offset);
                    mapping.TrimDownId = state.Offset.ToString();
                    assignTrimDownLabel.Text = mapping.TrimDownId;
                    run = false;

                }
            }

        }

        private void assignATCToggle_Click(object sender, EventArgs e)
        {
            assignATCToggleLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("ATC: " + state.Offset);
                    mapping.AtcMenuId = state.Offset.ToString();
                    assignATCToggleLabel.Text = mapping.AtcMenuId;
                    run = false;

                }
            }

        }

        private void assignLanding_Click(object sender, EventArgs e)
        {
            assignLandingLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Landing: " + state.Offset);
                    mapping.LandingId = state.Offset.ToString();
                    assignLandingLabel.Text = mapping.LandingId;
                    run = false;

                }
            }

        }

        private void assignNav_Click(object sender, EventArgs e)
        {
            assignNavLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Nav: " + state.Offset);
                    mapping.NavId = state.Offset.ToString();
                    assignNavLabel.Text = mapping.NavId;
                    run = false;

                }
            }

        }

        private void assignStrobe_Click(object sender, EventArgs e)
        {
            assignStrobeLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Strobe: " + state.Offset);
                    mapping.StrobeId = state.Offset.ToString();
                    assignStrobeLabel.Text = mapping.StrobeId;
                    run = false;

                }
            }

        }

        private void assignBeacon_Click(object sender, EventArgs e)
        {
            assignBeaconLabel.Text = "Press button on joystick...";

            var run = true;
            while (run == true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("Beacon: " + state.Offset);
                    mapping.BeaconId = state.Offset.ToString();
                    assignBeaconLabel.Text = mapping.BeaconId;
                    run = false;

                }
            }

        }

        private void cockpitCamButton_Click(object sender, EventArgs e)
        {
            commands.cockpitCamera();
        }

        private void vcCamButton_Click(object sender, EventArgs e)
        {
            commands.vcCamera();
        }

        private void followCamButton_Click(object sender, EventArgs e)
        {
            commands.followCamera();
        }

        private void onBoardCamButton_Click(object sender, EventArgs e)
        {
            commands.onboardCamera();
        }

        private void flybyCamButton_Click(object sender, EventArgs e)
        {
            commands.flybyCamera();
        }

        private void towerCamButton_Click(object sender, EventArgs e)
        {
            commands.towerCamera();
        }


        private void degreesChanged(object sender, System.EventArgs e)
        {
            heading = degreesTrackBar.Value;
            degreesLabel.Text = degreesTrackBar.Value + "°";

            commands.autopilotHeading(heading.ToString());
        }

        private void autopilotToggle_Click(object sender, EventArgs e)
        {
            commands.autopilot();
        }

        private void toggleHudtoggleHud_Click(object sender, EventArgs e)
        {
            commands.hud();
        }

        private void atcToggle_Click(object sender, EventArgs e)
        {
            commands.atcMenu();
        }

        private void pauseToggle_Click(object sender, EventArgs e)
        {
            commands.pause();
        }

        private void parkingBrakes_Click(object sender, EventArgs e)
        {
            commands.parkingBrake();
        }

        private void flapsDown_Click(object sender, EventArgs e)
        {
            commands.flapsDown();
        }

        private void flapsUp_Click(object sender, EventArgs e)
        {
            commands.flapsUp();
        }

        private void Spoilers_Click(object sender, EventArgs e)
        {
            commands.spoilers();
        }

        private void landingGear_Click(object sender, EventArgs e)
        {
            commands.landingGear();
        }

        private void pushback_Click(object sender, EventArgs e)
        {
            commands.pushback();
        }

        private void landing_Click(object sender, EventArgs e)
        {
            commands.landing();
        }

        private void strobe_Click(object sender, EventArgs e)
        {
            commands.strobe();
        }

        private void nav_Click(object sender, EventArgs e)
        {
            commands.nav();
        }

        private void beacon_Click(object sender, EventArgs e)
        {
            commands.beacon();
        }







        /*private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar = Keys.F1 && e.Alt == true))
            {
                //do something
            }
        }*/

    }
}
