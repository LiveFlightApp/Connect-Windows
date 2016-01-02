//
//  KeyboardCommandHandler.cs
//  LiveFlight Connect
//
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
using System.Windows.Input;
using System.Windows.Forms;

namespace LiveFlight
{
    class KeyboardCommandHandler
    {
 
        public static void keyPressed(Keys keyData)
        {

            var commands = MainWindow.commands;

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

        }


    }
}
