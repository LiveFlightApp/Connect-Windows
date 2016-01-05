//
//  
//  LiveFlight Connect
//
//  KeyboardCommandHandler.cs
//  Copyright © 2016 Cameron Carmichael Alonso. All rights reserved.
//
//  Licensed under GPL-V3.
//  https://github.com/LiveFlightApp/Connect-Windows/blob/master/LICENSE
//


using System.Windows.Input;

namespace LiveFlight
{
    class KeyboardCommandHandler
    {

        static bool shiftModifierDown = false;
        static Commands commands = MainWindow.commands;
 
        public static void keyPressed(System.Windows.Input.Key keyData)
        {

            // Check modifier keys
            if (keyData == Key.LeftShift || keyData == Key.RightShift)
            {
                shiftModifierDown = true;
            }


            //ATC
            if (keyData == System.Windows.Input.Key.D1)
            {
                commands.atc1();

            }
            else if (keyData == System.Windows.Input.Key.D2)
            {
                commands.atc2();

            }
            else if (keyData == System.Windows.Input.Key.D3)
            {
                commands.atc3();

            }
            else if (keyData == System.Windows.Input.Key.D4)
            {
                commands.atc4();

            }
            else if (keyData == System.Windows.Input.Key.D5)
            {
                commands.atc5();

            }
            else if (keyData == System.Windows.Input.Key.D6)
            {
                commands.atc6();

            }
            else if (keyData == System.Windows.Input.Key.D7)
            {
                commands.atc7();

            }
            else if (keyData == System.Windows.Input.Key.D8)
            {
                commands.atc8();

            }
            else if (keyData == System.Windows.Input.Key.D9)
            {
                commands.atc9();

            }
            else if (keyData == System.Windows.Input.Key.D0)
            {
                commands.atc10();

            }
            else if (keyData == System.Windows.Input.Key.A)
            {
                //toggle atc window
                commands.atcMenu();
            }

            //flight controls
            if (keyData == System.Windows.Input.Key.G)
            {
                //toggle landing gear
                commands.landingGear();
            }
            else if (keyData == System.Windows.Input.Key.P)
            {
                //shift + p
                //pushback
                commands.pushback();
            }
            else if (keyData == System.Windows.Input.Key.Space)
            {
                //pause
                commands.pause();

            }
            else if (keyData == System.Windows.Input.Key.OemPeriod)
            {
                //brake
                commands.parkingBrake();
            }
            else if (keyData == System.Windows.Input.Key.OemOpenBrackets)
            {
                //retract flaps
                commands.flapsUp();
            }

            else if (keyData == System.Windows.Input.Key.OemCloseBrackets)
            {
                //extend flaps
                commands.flapsDown();
            }
            else if (keyData == System.Windows.Input.Key.OemQuestion)
            {
                //spoilers
                commands.spoilers();
            }
            else if (keyData == System.Windows.Input.Key.L)
            {
                //kanding lights
                commands.landing();
            }
            else if (keyData == System.Windows.Input.Key.S)
            {
                //strobe
                commands.strobe();
            }
            else if (keyData == System.Windows.Input.Key.N)
            {
                //nav
                commands.nav();
            }
            else if (keyData == System.Windows.Input.Key.B)
            {
                //beacon
                commands.beacon();
            }
            else if (keyData == System.Windows.Input.Key.Z)
            {
                //toggle autopilot
                commands.autopilot();
            }
            else if (keyData == System.Windows.Input.Key.OemMinus)
            {
                //zoomout
                commands.zoomOut();
            }
            else if (keyData == System.Windows.Input.Key.OemPlus)
            {
                //zoomout
                commands.zoomIn();
            }
            else if ((keyData == System.Windows.Input.Key.Up) && shiftModifierDown)
            {
                //move up
                commands.movePOV(0);
                
            }
            else if ((keyData == System.Windows.Input.Key.Down) && shiftModifierDown)
            {
                //move down
                commands.movePOV(18000);

            }
            else if ((keyData == System.Windows.Input.Key.Left) && shiftModifierDown)
            {
                //move left
                commands.movePOV(27000);

            }
            else if ((keyData == System.Windows.Input.Key.Right) && shiftModifierDown)
            {
                //move right
                commands.movePOV(9000);

            }
            else if (keyData == System.Windows.Input.Key.Up)
            {
                //move pitch down
                commands.pitchDown();

            }
            else if (keyData == System.Windows.Input.Key.Down)
            {
                //move pitch up
                commands.pitchUp();

            }
            else if (keyData == System.Windows.Input.Key.Left)
            {
                //move roll left
                commands.rollLeft();

            }
            else if (keyData == System.Windows.Input.Key.Right)
            {
                //move roll right
                commands.rollRight();

            }
            else if (keyData == System.Windows.Input.Key.D)
            {
                //increase throttle
                commands.increaseThrottle();
                
            }
            else if (keyData == System.Windows.Input.Key.C)
            {
                //decrease throttle
                commands.decreaseThrottle();

            }
            else if (keyData == System.Windows.Input.Key.E)
            {
                //Next Camera
                commands.nextCamera();
            }
            else if (keyData == System.Windows.Input.Key.Q)
            {
                //previous Camera
                commands.previousCamera();
            }


        }

        // key UP
        public static void keyUp(System.Windows.Input.Key keyData)
        {

            // stop moving POV
            commands.movePOV(-1);

            if (keyData == Key.LeftShift || keyData == Key.RightShift)
            {
                shiftModifierDown = false;
            }

        }


    }
}
