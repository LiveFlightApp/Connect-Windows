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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiveFlight
{
    class KeyboardCommandHandler
    {
 
        public static void keyPressed(System.Windows.Input.Key keyData)
        {

            var commands = MainWindow.commands;

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
            else if (keyData == (System.Windows.Input.Key.Up | System.Windows.Input.Key.LeftShift | System.Windows.Input.Key.RightShift))
            {
                //move up
                commands.movePOV(0);
                
            }
            else if (keyData == (System.Windows.Input.Key.Down | System.Windows.Input.Key.LeftShift | System.Windows.Input.Key.RightShift))
            {
                //move down
                commands.movePOV(18000);

            }
            else if (keyData == (System.Windows.Input.Key.Left | System.Windows.Input.Key.LeftShift | System.Windows.Input.Key.RightShift))
            {
                //move left
                commands.movePOV(27000);

            }
            else if (keyData == (System.Windows.Input.Key.Right | System.Windows.Input.Key.LeftShift | System.Windows.Input.Key.RightShift))
            {
                //move right
                commands.movePOV(9000);

            }
            else if (keyData == System.Windows.Input.Key.Up)
            {
                //move pitch down
                

            }
            else if (keyData == System.Windows.Input.Key.Down)
            {
                //move pitch up
                

            }
            else if (keyData == System.Windows.Input.Key.Left)
            {
                //move roll left
                

            }
            else if (keyData == System.Windows.Input.Key.Right)
            {
                //move roll right
                

            }
            else if (keyData == System.Windows.Input.Key.D)
            {
                //increase throttle
                
            }
            else if (keyData == System.Windows.Input.Key.C)
            {
                //decrease throttle

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


    }
}
