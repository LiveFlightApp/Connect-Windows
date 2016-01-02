//
//  
//  LiveFlight Connect
//
//  commands.cs
//  Copyright © 2016 Cameron Carmichael Alonso. All rights reserved.
//
//  Licensed under GPL-V3.
//  https://github.com/LiveFlightApp/Connect-Windows/blob/master/LICENSE
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fds.IFAPI;

namespace LiveFlight
{
    public class Commands
    {
        IFConnectorClient client = LiveFlight.MainWindow.client;

        int viewUpId = 0;
        int viewDownId = 18000;
        int viewLeftId = 27000;
        int viewRightId = 9000;


        public void previousCamera()
        {
            Console.WriteLine("Moving to previous camera...");
            client.ExecuteCommand("Commands.PrevCamera");
        }

        public void nextCamera()
        {
            Console.WriteLine("Moving to next camera...");
            client.ExecuteCommand("Commands.NextCamera");
        }

        public void cockpitCamera()
        {
            Console.WriteLine("Moving to cockpit camera...");
            client.ExecuteCommand("Commands.SetCockpitCamera");
        }

        public void vcCamera()
        {
            Console.WriteLine("Moving to virtual cockpit camera...");
            client.ExecuteCommand("Commands.SetVirtualCockpitCameraCommand");
        }

        public void followCamera()
        {
            Console.WriteLine("Moving to follow camera...");
            client.ExecuteCommand("Commands.SetFollowCameraCommand");
        }

        public void onboardCamera()
        {
            Console.WriteLine("Moving to onboard camera...");
            client.ExecuteCommand("Commands.SetOnboardCameraCommand");
        }

        public void towerCamera()
        {
            Console.WriteLine("Moving to tower camera...");
            client.ExecuteCommand("Commands.SetTowerCameraCommand");
        }

        public void flybyCamera()
        {
            Console.WriteLine("Moving to flyby camera...");
            client.ExecuteCommand("Commands.SetFlyByCamera");
        }

        public void flapsDown()
        {
            Console.WriteLine("Moving flaps down...");
            client.ExecuteCommand("Commands.FlapsDown");
        }

        public void flapsUp()
        {
            Console.WriteLine("Moving flaps up...");
            client.ExecuteCommand("Commands.FlapsUp");
        }

        public void landingGear()
        {
            Console.WriteLine("Toggling landing gear...");
            client.ExecuteCommand("Commands.LandingGear");
        }

        public void spoilers()
        {
            Console.WriteLine("Toggling spoilers...");
            client.ExecuteCommand("Commands.Spoilers");
        }


        public void movePOV(int value)
        {
            //POV axis
            var xValue = 0;
            var yValue = 0;

            Console.WriteLine(value);

            if (value == viewUpId)
            {
                xValue = 0;
                yValue = -1;
            }
            else if (value == viewDownId)
            {
                xValue = 0;
                yValue = 1;
            }
            else if (value == viewLeftId)
            {
                xValue = -1;
                yValue = 0;
            }
            else if (value == viewRightId)
            {
                xValue = 1;
                yValue = 0;
            }

            Console.WriteLine(xValue + "  " + yValue);

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

        public void reverseThrust()
        {
            Console.WriteLine("Toggling reverse thrust...");
            client.ExecuteCommand("Commands.ReverseThrust");
        }

        public void autopilot()
        {
            Console.WriteLine("Toggling autopilot...");
            client.ExecuteCommand("Commands.Autopilot.Toggle");
        }

        public void hud()
        {
            Console.WriteLine("Toggling HUD...");
            client.ExecuteCommand("Commands.ToggleHUD");
        }

        public void parkingBrake()
        {
            Console.WriteLine("Toggling parking brake...");
            client.ExecuteCommand("Commands.ParkingBrakes");
        }

        public void pause()
        {
            Console.WriteLine("Toggling pause...");
            client.ExecuteCommand("Commands.TogglePause");
        }

        public void pushback()
        {
            Console.WriteLine("Toggling pushback...");
            client.ExecuteCommand("Commands.Pushback");
        }

        public void trimUp()
        {
            Console.WriteLine("Trim up...");
            client.ExecuteCommand("Commands.ElevatorTrimUp");
        }

        public void trimDown()
        {
            Console.WriteLine("Trim down...");
            client.ExecuteCommand("Commands.ElevatorTrimDown");
        }

        public void atcMenu()
        {
            Console.WriteLine("Toggling ATC Menu...");
            client.ExecuteCommand("Commands.ShowATCWindowCommand");
        }

        public void landing()
        {
            Console.WriteLine("Toggling Landing Lights...");
            client.ExecuteCommand("Commands.LandingLights");
        }

        public void nav()
        {
            Console.WriteLine("Toggling Nav Lights...");
            client.ExecuteCommand("Commands.NavLights");
        }

        public void strobe()
        {
            Console.WriteLine("Toggling Strobes...");
            client.ExecuteCommand("Commands.StrobeLights");
        }

        public void beacon()
        {
            Console.WriteLine("Toggling Beacon...");
            client.ExecuteCommand("Commands.BeaconLights");
        }

        public void autopilotHeading(string heading)
        {
            Console.WriteLine("Changing heading to {0}...", heading);
            client.ExecuteCommand("Commands.Autopilot.SetHeading", new CallParameter[] { new CallParameter { Name = "Heading", Value = heading } });
        }

        public void atc1()
        {
            Console.WriteLine("ATC command...");
            client.ExecuteCommand("Commands.ATCEntry1");
        }

        public void atc2()
        {
            Console.WriteLine("ATC command...");
            client.ExecuteCommand("Commands.ATCEntry2");
        }

        public void atc3()
        {
            Console.WriteLine("ATC command...");
            client.ExecuteCommand("Commands.ATCEntry3");
        }

        public void atc4()
        {
            Console.WriteLine("ATC command...");
            client.ExecuteCommand("Commands.ATCEntry4");
        }

        public void atc5()
        {
            Console.WriteLine("ATC command...");
            client.ExecuteCommand("Commands.ATCEntry5");
        }

        public void atc6()
        {
            Console.WriteLine("ATC command...");
            client.ExecuteCommand("Commands.ATCEntry6");
        }

        public void atc7()
        {
            Console.WriteLine("ATC command...");
            client.ExecuteCommand("Commands.ATCEntry7");
        }

        public void atc8()
        {
            Console.WriteLine("ATC command...");
            client.ExecuteCommand("Commands.ATCEntry8");
        }

        public void atc9()
        {
            Console.WriteLine("ATC command...");
            client.ExecuteCommand("Commands.ATCEntry9");
        }

        public void atc10()
        {
            Console.WriteLine("ATC command...");
            client.ExecuteCommand("Commands.ATCEntry10");
        }

        public void zoomOut()
        {
            Console.WriteLine("Zoom out...");
            client.ExecuteCommand("Commands.CameraZoomOut");
        }

        public void zoomIn()
        {
            Console.WriteLine("Zoom in...");
            client.ExecuteCommand("Commands.CameraZoomIn");
        }
    }
}
