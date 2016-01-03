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

        Commands commands = new Commands();
        DirectInput directInput = new DirectInput();

        // count number of joysticks and gamepads
        int gamepadCount = 0;
        int joystickCount = 0;

        public void beginJoystickPoll()
        {

            // search for gamepads
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad,
                        DeviceEnumerationFlags.AllDevices))
            {
                // poll async
                Task.Run(() =>
                {
                    pollJoystick(deviceInstance.InstanceGuid);
                });

                gamepadCount += 1;
            }

            // search for joysticks
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,
                    DeviceEnumerationFlags.AllDevices))
            {
                // poll async
                Task.Run(() =>
                {
                    pollJoystick(deviceInstance.InstanceGuid);
                });

                joystickCount += 1;
            }

            // check that devices definitely exist
            if (gamepadCount == 0 && joystickCount == 0)
            {
                Console.WriteLine("No joystick found, assuming keyboard keys will be used.");
            }
            else
            {
                Console.WriteLine("Found {0} joystick(s) and {1} gamepad(s)", joystickCount, gamepadCount);
            }


        }

        private void pollJoystick(Guid joystickGuid)
        {

            Joystick joystick;

            // Instantiate the joystick
            joystick = new Joystick(directInput, joystickGuid);
            joystick.Properties.BufferSize = 128;

            Console.WriteLine("Joystick {0} with GUID: {1}", joystick.Properties.ProductName, joystickGuid);

            // Query all suported ForceFeedback effects
            // TODO - maybe look into vibration effects on gamepads?
            var allEffects = joystick.GetEffects();
            foreach (var effectInfo in allEffects)
            {
                Console.WriteLine("Effect available {0}", effectInfo.Name);
            }

            // Acquire the joystick
            joystick.Acquire();

            // Poll events from joystick
            while (true)
            {
                joystick.Poll();
                var data = joystick.GetBufferedData();
                foreach (var state in data)
                {
                    Console.WriteLine("{0} - {1} - {2}", joystick.Properties.ProductName, state.Offset, state.Value);

                    if (state.Offset.ToString().StartsWith("Button") || state.Offset.ToString().StartsWith("Point"))
                    {

                        buttonPressed(state.Offset, state.Value);

                    }
                    else
                    {

                        //is an axis
                        int range = 32767; // this is for main joysticks. Might have to change if there are issues
                        axisMoved(state.Offset, state.Value, range);

                    }

                }
            }

        }

        private void buttonPressed(JoystickOffset offset, int value)
        {

            String state;
            
            // check button state based on value
            // this is the inverse of OS X, where != 0 is up
            if (value == 0)
            {
                state = "Up";
            }
            else
            {
                state = "Down";
            }

            int offsetValue = Int32.Parse(offset.ToString().Replace("Buttons", "")); // leave just a number
            commands.joystickButtonChanged(offsetValue, state);

        }

        private void axisMoved(JoystickOffset offset, int value, int range)
        {

            //indexes
            // 0 - pitch
            // 1 - roll
            // 2 - rudder
            // 3 - throttle

            // do some calculations to make it closer to [-1024, 1024]
            value -= range;
            value = value / 32;

            if (offset.ToString() == "X")
            {

                commands.movedJoystickAxis(1, value);

            }
            else if (offset.ToString() == "Y")
            {

                commands.movedJoystickAxis(0, value);

            }
            else if (offset.ToString() == "RotationZ")
            {
                // assumes twisted rudder

                commands.movedJoystickAxis(2, value);

            }
            else if ((offset.ToString() == "Z") || (offset.ToString().StartsWith("Slider")))
            {
                // assumes a slider is for throttle
                // this might not be the case on the T. Flight Hotas where it is also yaw, but this should do for sticks like the 3D Pro.

                commands.movedJoystickAxis(3, value);

            }
            


        }

    }
}
