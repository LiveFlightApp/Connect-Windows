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

        // store GUIDs in array to check for removal
        List<Guid> gamepads = new List<Guid>();
        List<Guid> joysticks = new List<Guid>();

        // POV values
        int viewUpId = 0;
        int viewDownId = 18000;
        int viewLeftId = 27000;
        int viewRightId = 9000;

        int lastPovId = 0;

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
                gamepads.Add(deviceInstance.InstanceGuid);
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
                joysticks.Add(deviceInstance.InstanceGuid);
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

            Console.WriteLine("Joystick {0}", joystick.Properties.ProductName.ToString());
            Console.WriteLine("GUID: {0}", joystickGuid);

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
                    //Null characters in the device name clogs up the write buffer when not run as a Console app. Replace all \0 with empty string to fix this.
                    Console.WriteLine("{0} - {1} - {2}", joystick.Properties.InstanceName.Replace("\0", ""), state.Offset, state.Value);

                    if (state.Offset.ToString().StartsWith("Button"))
                    {

                        buttonPressed(state.Offset, state.Value);
                        Console.WriteLine("Button {0} state changed to {1}", state.Offset, state.Value);

                    }
                    else if (state.Offset.ToString().StartsWith("Point"))
                    {

                        // POV controller
                        povChanged(state.Offset, state.Value);

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

        private void povChanged(JoystickOffset offset, int value)
        {

            String state;
            int offsetValue = lastPovId; // prefixed by 11 to avoid duplicates with standard buttons

            // set offset depending on direction
            if (value == viewUpId)
            {
                offsetValue = 111;
            }
            else if (value == viewRightId)
            {
                offsetValue = 112;
            }
            else if (value == viewDownId)
            {
                offsetValue = 113;
            }
            else if (value == viewLeftId)
            {
                offsetValue = 114;
            }

            // set lastPovId
            lastPovId = offsetValue;

            // check POV state
            if (value == -1)
            {
                state = "Up";
            }
            else
            {
                state = "Down";
            }

            
            commands.joystickButtonChanged(Int32.Parse(offsetValue.ToString()), state);

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

        public void deviceRemoved()
        {

            // check connected devices
            // search for gamepads

            List<Guid> currentGamepads = new List<Guid>();
            List<Guid> currentJoysticks = new List<Guid>();

            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Gamepad,
                        DeviceEnumerationFlags.AllDevices))
            {
                currentGamepads.Add(deviceInstance.InstanceGuid);
            }

            // search for joysticks
            foreach (var deviceInstance in directInput.GetDevices(DeviceType.Joystick,
                    DeviceEnumerationFlags.AllDevices))
            {
                currentJoysticks.Add(deviceInstance.InstanceGuid);
            }

            // iterate through past gamepads, compare with current
            foreach (var gamepad in gamepads)
            {
                if (!currentGamepads.Contains(gamepad))
                {
                    // device removed
                    gamepads.Remove(gamepad);
                    gamepadCount -= 1;
                    return;
                }
            }

            // iterate through past joysticks, compare with current
            foreach (var joystick in joysticks)
            {
                if (!currentJoysticks.Contains(joystick))
                {
                    // device removed
                    currentJoysticks.Remove(joystick);
                    joystickCount -= 1;
                    Console.WriteLine("Removed joystick {0}", joystick);
                    return;
                }
            }
        }

    }
}
