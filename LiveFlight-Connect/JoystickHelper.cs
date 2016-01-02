//
//  
//  LiveFlight Connect
//
//  JoystickHelper.cs
//  Copyright © 2015 Cameron Carmichael Alonso. All rights reserved.
//
//  Licensed under GPL-V3.
//  https://github.com/LiveFlightApp/Connect-Windows/blob/master/LICENSE
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace LiveFlight
{
    class JoystickHelper
    {

        Joystick joystick;

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

                            // auto ap disable

                            /*if (axisMoveAutopilotDisable.Checked == true)
                            {
                                if (isApOn == true)
                                {
                                    Console.WriteLine("Disable AP");
                                    commands.autopilot();
                                    isApOn = false;
                                }

                            }*/
                           

                        }

                        //is an axis
                        axisMoved(state.Offset, state.Value, 32767);

                    }

                }
            }

        }

        private void buttonPressed(JoystickOffset offset, int value)
        {

            // send button offset

        }

        private void axisMoved(JoystickOffset offset, int value, int range)
        {

            // TODO - this whole section can be rewritten. Duplicate work, assigning value then checking against

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

            // check if is joystick movement
            if (indexOf < 2)
            {
                //divide value by 10
                value = value - range;
                value = value / 32;

            }
            else
            {

                // Throttle
                if (indexOf == 3)
                {

                    value = value - range;
                    value = value / 32;

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

                /*client.ExecuteCommand("NetworkJoystick.SetAxisValue", new CallParameter[]
                {
                new CallParameter
                {
                    Name = indexOf.ToString(),
                    Value = value.ToString()
                }
                });*/

            }

        }

    }
}
