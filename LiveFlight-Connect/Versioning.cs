//
//  
//  LiveFlight Connect
//
//  Versioning.cs
//  Copyright © 2015 Cameron Carmichael Alonso. All rights reserved.
//
//  Licensed under GPL-V3.
//  https://github.com/LiveFlightApp/Connect-Windows/blob/master/LICENSE
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;

namespace LiveFlight
{
    class Versioning
    {

        static String configURL = "http://connect.liveflightapp.com/config/config.json";
        static String updateURL = "http://connect.liveflightapp.com/update/windows";

        // set app version
        // done here instead of embedded in assembly meta so that it works with server-side versioning
        public static double currentAppVersion = 1.0;

        public static void checkForUpdate()
        {

            VersioningFile windowsVersions = Serializer.DeserializeJson<VersioningFile>(makeGetRequest());

            // make sure newest version is at index 0
            List<VersionHistory> versionHistorySorted = windowsVersions.windows.OrderByDescending(o => o.version).ToList();

            double newVersion = versionHistorySorted[0].version;
            String newVersionLog = versionHistorySorted[0].log;

            if (newVersion > currentAppVersion)
            {
                // an update is available:
                Console.WriteLine("New version {0} available!\n{1}\n\n", newVersion, newVersionLog);

                String titleString = String.Format("An update is available");
                String messageString = String.Format("Version {0} is now available!\n\nChangelog:\n{1}\n\nTap OK to visit update website.", newVersion, newVersionLog);

                MessageBoxResult result = MessageBox.Show(messageString, titleString, MessageBoxButton.OK);
                if (result == MessageBoxResult.OK)
                {
                    System.Diagnostics.Process.Start(updateURL);
                    Application.Current.Shutdown();
                }

            }


        }

        private static string makeGetRequest()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(configURL);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);

                string data = reader.ReadToEnd();

                reader.Close();
                stream.Close();

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Encountered an exception whilst making get request: {0}", e);
            }

            return null;
        }

        [DataContract]
        public class VersioningFile
        {
            [DataMember]
            public List<VersionHistory> windows { get; set; }
        }

        [DataContract]
        public class VersionHistory
        {
            [DataMember]
            public double version { get; set; }

            [DataMember]
            public String log { get; set;  }
        }

    }
}
