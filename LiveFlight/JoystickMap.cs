using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LiveFlight
{
    public class JoystickMap
    {
        String fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".lf.mapping");

        private String brakesButtonId;
        private String previousButtonId;
        private String nextButtonId;
        private String flapDownId;
        private String flapUpId;
        private String gearId;
        private String spoilersId;
        private String reverseId;
        private String autopilotId;
        private String hudId;
        private String parkingBrakeId;
        private String pushbackId;
        private String pauseId;
        private String trimUpId;
        private String trimDownId;
        private String atcMenuId;
        private String landingId;
        private String beaconId;
        private String strobeId;
        private String navId;


        public String BrakesButtonId
        {
            get
            {
                return brakesButtonId;
            }
            set
            {
                brakesButtonId = value;
                changed();
            }

        }

        public String PreviousButtonId
        {
            get
            {
                return previousButtonId;
            }
            set
            {
                previousButtonId = value;
                changed();
            }

        }

        public String NextButtonId
        {
            get
            {
                return nextButtonId;
            }
            set
            {
                nextButtonId = value;
                changed();
            }

        }

        public String FlapDownId
        {
            get
            {
                return flapDownId;
            }
            set
            {
               flapDownId = value;
                changed();
            }

        }

        public String FlapUpId
        {
            get
            {
                return flapUpId;
            }
            set
            {
                flapUpId = value;
                changed();
            }

        }

        public String GearId
        {
            get
            {
                return gearId;
            }
            set
            {
                gearId = value;
                changed();
            }

        }

        public String SpoilersId
        {
            get
            {
                return spoilersId;
            }
            set
            {
                spoilersId = value;
                changed();
            }

        }

        public String ReverseId
        {
            get {
                return reverseId;
            }
            set {
                reverseId = value;
                changed();
            }

        }

        public String AutopilotId
        {
            get
            {
                return autopilotId;
            }
            set
            {
                autopilotId = value;
                changed();
            }

        }

        public String HudId
        {
            get
            {
                return hudId;
            }
            set
            {
                hudId = value;
                changed();
            }

        }

        public String ParkingBrakeId
        {
            get
            {
                return parkingBrakeId;
            }
            set
            {
                parkingBrakeId = value;
                changed();
            }

        }

        public String PushbackId
        {
            get
            {
                return pushbackId;
            }
            set
            {
                pushbackId = value;
                changed();
            }

        }

        public String PauseId
        {
            get
            {
                return pauseId;
            }
            set
            {
                pauseId = value;
                changed();
            }

        }

        public String TrimUpId
        {
            get
            {
                return trimUpId;
            }
            set
            {
                trimUpId = value;
                changed();
            }

        }
        public String TrimDownId
        {
            get
            {
                return trimDownId;
            }
            set
            {
                trimDownId = value;
                changed();
            }

        }

        public String AtcMenuId
        {
            get
            {
                return atcMenuId;
            }
            set
            {
                atcMenuId = value;
                changed();
            }

        }

        public String LandingId
        {
            get
            {
                return landingId;
            }
            set
            {
                landingId = value;
                changed();
            }

        }

        public String NavId
        {
            get
            {
                return navId;
            }
            set
            {
                navId = value;
                changed();
            }

        }

        public String StrobeId
        {
            get
            {
                return strobeId;
            }
            set
            {
               strobeId = value;
                changed();
            }

        }

        public String BeaconId
        {
            get
            {
                return beaconId;
            }
            set
            {
                beaconId = value;
                changed();
            }

        }

        private void changed()
        {
            //save
            string output = Fds.IFAPI.Serializer.SerializeJson<JoystickMap>(this);
            byte[] bytes = GetBytes(output);

            Console.WriteLine("Saving map file to {0}", fileName);
            File.WriteAllBytes(fileName, bytes);
            //File.WriteAllText(fileName, output);

            //open();
        }


        public void open()
        {
            try {

                byte[] bytes = File.ReadAllBytes(fileName);
                string values = GetString(bytes);

                var obj = Fds.IFAPI.Serializer.DeserializeJson<JoystickMap>(values);
                BrakesButtonId = obj.BrakesButtonId;
                FlapUpId = obj.FlapUpId;
                FlapDownId = obj.FlapDownId;
                PreviousButtonId = obj.PreviousButtonId;
                NextButtonId = obj.NextButtonId;
                GearId = obj.GearId;
                SpoilersId = obj.SpoilersId;
                ReverseId = obj.ReverseId;
                AutopilotId = obj.AutopilotId;
                HudId = obj.HudId;
                ParkingBrakeId = obj.ParkingBrakeId;
                PushbackId = obj.PushbackId;
                PauseId = obj.PauseId;
                TrimUpId = obj.TrimUpId;
                TrimDownId = obj.TrimDownId;
                AtcMenuId = obj.AtcMenuId;
                LandingId = obj.LandingId;
                NavId = obj.NavId;
                StrobeId = obj.StrobeId;
                BeaconId = obj.BeaconId;

            } catch (System.Exception ex)
            {
                Console.WriteLine("Caught exception: {0}", ex);

            }

        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

    }
}
