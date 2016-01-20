# LiveFlight Connect - Windows
LiveFlight Connect for Windows allows you to control Infinite Flight on an iOS or Android device via your PC using either joystick, keyboard, or mouse. 

Uses Infinite Flight Connect, a TCP-based API introduced in version 15.10.0.

![](https://raw.githubusercontent.com/LiveFlightApp/Connect-Windows/master/screenshot.png "LiveFlight Connect for Windows")

Usage
------------
  * Install the latest release version from [connect.liveflightapp.com](http://connect.liveflightapp.com)
  * Enable Infinite Flight Connect within Infinite Flight, and make sure your device is on the same wifi network as your PC
  * Get your joystick setup, and go have fun :)


Modifying Source
------------
LiveFlight Connect is built in C#. Simply clone the repo, run and build in Visual Studio! 

Compatible Devices
------------
There's no guarantee this will play perfectly with your joystick or configuration. These joysticks definitely work fine:
  * Thrustmaster T-Flight Hotas X
  * Saitek X52 Pro
  * Logitech Extreme 3D
   
  Gamepads might not work perfectly at this time.


Project Descriptions
------------
1. **LiveFlight-Connect**:
Main project - handles most of the UI, joystick work, keyboard commands.
2. **IFConnect**:
This project contains the interfaces for communicating with the IFConnect API and datatypes for deserializing JSON data.
3. **IF_FMS**: 
This project handles the FMS system for AutoNav, as well as the logic controlling it.
4. **FlightPlanDatabase**:
This contains some interfaces for FlightPlanDatabase.com API. Simple flight plan datatypes and logic implemented so far to search for flight plans.
5. **Indicators**:
Work in progress. Started with an attitude indicator that still needs some refinement. 
Wanted to use an image as the background scale but can't get it to position correctly to match pitch.

 
LiveFlight Connect License
-----------
Licensed under the GPL-V3 License <a href="https://github.com/LiveFlightApp/Connect-Windows/blob/master/LICENSE">available here</a>.
