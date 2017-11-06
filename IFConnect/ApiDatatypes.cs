using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// IFConnect API datatypes.
/// </summary>
/// <remarks>
/// </remarks>
namespace Fds.IFAPI
{
    [DataContract]
    public class ATCMessage
    {
        [DataMember]
        public string Text { get; set; }
    }

    [DataContract]
    public class ATCMessageList : APIResponse
    {
        [DataMember]
        public ATCMessage[] ATCMessages { get; set; }
    }

    [DataContract]
    public class APIWaypoint
    {
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Code { get; set; }
    }

    [DataContract]
    public class APIFlightPlan : APIResponse
    {
        [DataMember]
        public APIWaypoint[] Waypoints { get; set; }
    }

    [DataContract]
    public class APIFrequencyInfo
    {
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public int IntegerFrequency { get; set; }
        [DataMember]
        public Guid FrequencyID { get; set; }
        [DataMember]
        public string FacilityCode { get; set; }
    }

    [DataContract]
    public class APIFrequencyInfoList : APIResponse
    {
        [DataMember]
        public APIFrequencyInfo[] Frequencies { get; set; }
    }

    [DataContract]
    public class APIServerInfo
    {
        [DataMember]
        public string[] Addresses { get; set; }
        [DataMember]
        public int Port { get; set; }
    }

    [DataContract]
    public class IFAPIStatus : APIResponse
    {
        [DataMember]
        public string AppVersion { get; set; }
        [DataMember]
        public string ApiVersion { get; set; }
        [DataMember]
        public string LoggedInUser { get; set; }
        [DataMember]
        public int DisplayWidth { get; set; }
        [DataMember]
        public int DisplayHeight { get; set; }
        [DataMember]
        public string DeviceName { get; set; }
    }

    [DataContract]
    public class APIATCMessage : APIResponse
    {
        [DataMember]
        public bool Received { get; set; }
        [DataMember]
        public Guid EmitterID { get; set; }
        [DataMember]
        public string EmitterUserName { get; set; }
        [DataMember]
        public string EmitterCallsign { get; set; }
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public string SynthesizableMessage { get; set; }
        [DataMember]
        public Guid FrequencyID { get; set; }
        [DataMember]
        public string FrequencyName { get; set; }
    }

    [DataContract]
    public enum APIResult
    {
        [EnumMember]
        OK,
        [EnumMember]
        Error
    }

    [DataContract(Namespace = "")]
    [KnownType(typeof(APIAircraftState))]
    public class APIResponse
    {
        [DataMember]
        public APIResult Result { get; set; }
        [DataMember]
        public string Type { get; set; }

        public APIResponse()
        {
            Type = this.GetType().ToString();
        }
    }

    [DataContract]
    public class FacilityList : APIResponse
    {
        [DataMember]
        public FacilityInfo[] Facilities { get; set; }
    }

    [DataContract]
    public class FacilityInfo
    {
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int IntegerFrequency { get; set; }
        [DataMember]
        public string AirportName { get; set; }
        [DataMember]
        public Guid ID { get; set; }
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public Guid UserID { get; set; }
        [DataMember]
        public DateTime StartTimeUTC { get; set; }
    }

    [DataContract]
    public class AirplaneInfo
    {
        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        [DataMember]
        public float Altitude { get; set; }

        [DataMember]
        public float VerticalSpeed { get; set; }

        [DataMember]
        public double Heading { get; set; }

        [DataMember]
        public double Velocity { get; set; }

        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string CallSign { get; set; }

        [DataMember]
        public Guid AircraftID { get; set; }

        [DataMember]
        public Guid LiveryID { get; set; }

        [DataMember]
        public string DeviceName { get; set; }

        [DataMember]
        public Guid FlightID { get; set; }

        [DataMember]
        public string AppVersion { get; set; }
    }

    [DataContract]
    public class LiveAirplaneList : APIResponse
    {
        [DataMember]
        public AirplaneInfo[] Airplanes { get; set; }
    }

    [DataContract]
    public class GetValueResponse : APIResponse
    {
        [DataMember]
        public CallParameter[] Parameters { get; set; }
    }

    public class Coordinate
    {
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public override string ToString()
        {
            return "(" + Math.Round(Latitude, 4) + "," + Math.Round(Longitude, 4) + ")";
        }
    }

    [DataContract(Namespace = "")]
    public class CallParameter
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value { get; set; }
    }

    [DataContract(Namespace = "")]
    public class APICall
    {
        [DataMember]
        public string Command { get; set; }

        [DataMember]
        public CallParameter[] Parameters { get; set; }
    }

    [DataContract]
    public enum GearState
    {
        [EnumMember]
        Unknown,
        [EnumMember]
        Down,
        [EnumMember]
        Up,
        [EnumMember]
        Moving,
        [EnumMember]
        MovingDown,
        [EnumMember]
        MovingUp
    }

    [DataContract]
    public enum SpoilersPosition
    {
        [EnumMember]
        Retracted,
        [EnumMember]
        Flight,
        [EnumMember]
        Full
    }

    [DataContract(Namespace = "")]
    public class APIAircraftState : APIResponse
    {
        [DataMember]
        public float AltitudeAGL { get; set; }
        [DataMember]
        public float AltitudeMSL { get; set; }
        [DataMember]
        public float Bank { get; set; }
        [DataMember]
        public float CourseTrue { get; set; }
        [DataMember]
        public GearState GearState { get; set; }
        [DataMember]
        public float GForce { get; set; }
        [DataMember]
        public float GroundSpeed { get; set; }
        [DataMember]
        public float GroundSpeedKts { get; set; }
        [DataMember]
        public float HeadingMagnetic { get; set; }
        [DataMember]
        public float HeadingTrue { get; set; }
        [DataMember]
        public float IndicatedAirspeed { get; set; }
        [DataMember]
        public float IndicatedAirspeedKts { get; set; }
        [DataMember]
        public bool IsAutopilotOn { get; set; }
        [DataMember]
        public bool IsCrashed { get; set; }
        [DataMember]
        public bool IsLanded { get; set; }
        [DataMember]
        public bool IsOnGround { get; set; }
        [DataMember]
        public bool IsOverLandingWeight { get; set; }
        [DataMember]
        public bool IsOverTakeoffWeight { get; set; }
        [DataMember]
        public bool IsPushbackActive { get; set; }
        [DataMember]
        public Coordinate Location { get; set; }
        [DataMember]
        public float MachNumber { get; set; }
        [DataMember]
        public float MagneticDeviation { get; set; }
        [DataMember]
        public float Pitch { get; set; }
        [DataMember]
        public bool ReverseThrustState { get; set; }
        [DataMember]
        public float SideForce { get; set; }
        [DataMember]
        public SpoilersPosition SpoilersPosition { get; set; }
        [DataMember]
        public bool Stalling { get; set; }
        [DataMember]
        public float StallProximity { get; set; }
        [DataMember]
        public bool StallWarning { get; set; }
        [DataMember]
        public float TrueAirspeed { get; set; }
        [DataMember]
        public float Velocity { get; set; }
        [DataMember]
        public float VerticalSpeed { get; set; }
        [DataMember]
        public float Weight { get; set; }
        [DataMember]
        public float WeightPercentage { get; set; }
    }

    [DataContract]
    public class APIAutopilotState : APIResponse
    {
        [DataMember]
        public bool EnableBankAngle { get; set; }
        [DataMember]
        public bool EnableHeading { get; set; }
        [DataMember]
        public bool EnableClimbRate { get; set; }
        [DataMember]
        public bool EnableAltitude { get; set; }
        [DataMember]
        public bool EnableSpeed { get; set; }
        [DataMember]
        public float TargetHeading { get; set; }
        [DataMember]
        public float TargetClimbRate { get; set; }
        [DataMember]
        public float TargetAltitude { get; set; }
        [DataMember]
        public float TargetSpeed { get; set; }
        [DataMember]
        public bool EnableApproach { get; set; }
    }



}
