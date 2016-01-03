using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightPlanDatabase
{
    public class ApiDataTypes
    {
        public class User
        {
            public int id { get; set; }
            public string username { get; set; }
            public string gravatarHash { get; set; }
            public object location { get; set; }
        }

        public class FlightPlanSummary
        {
            public int id { get; set; }
            public string fromICAO { get; set; }
            public string toICAO { get; set; }
            public string fromName { get; set; }
            public string toName { get; set; }
            public object flightNumber { get; set; }
            public double distance { get; set; }
            public int maxAltitude { get; set; }
            public int waypoints { get; set; }
            public int likes { get; set; }
            public int downloads { get; set; }
            public double popularity { get; set; }
            public string notes { get; set; }
            public string encodedPolyline { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
            public List<string> tags { get; set; }
            public User user { get; set; }
        }

        public class Via
        {
            public string type { get; set; }
            public string ident { get; set; }
        }

        public class Node
        {
            public string type { get; set; }
            public string ident { get; set; }
            public string name { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
            public int alt { get; set; }
            public Via via { get; set; }
        }

        public class Route
        {
            public List<Node> nodes { get; set; }
        }

        public class FlightPlanDetails
        {
            public int id { get; set; }
            public string fromICAO { get; set; }
            public string toICAO { get; set; }
            public string fromName { get; set; }
            public string toName { get; set; }
            public object flightNumber { get; set; }
            public double distance { get; set; }
            public int maxAltitude { get; set; }
            public int waypoints { get; set; }
            public int likes { get; set; }
            public int downloads { get; set; }
            public double popularity { get; set; }
            public string notes { get; set; }
            public string encodedPolyline { get; set; }
            public string createdAt { get; set; }
            public string updatedAt { get; set; }
            public List<string> tags { get; set; }
            public object user { get; set; }
            public Route route { get; set; }
        }

        public static List<T> deserializeJsonArray<T>(string jsonString)
        {
            Newtonsoft.Json.Linq.JArray jarr = Newtonsoft.Json.Linq.JArray.Parse(jsonString);
            List<T> lst = new List<T>();
            foreach(var j in jarr){
                lst.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<T>(j.ToString()));
            }
            return lst;
        }

        public static T deserializeJson<T>(string jsonString)
        {
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
            
            return t;
        }


    }
}
