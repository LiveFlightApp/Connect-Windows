using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace FlightPlanDatabase
{
    public class FpdApi
    {
        private const string _API_URI = "https://api.flightplandatabase.com/";
        
        /// <summary>
        /// Query API.
        /// </summary>
        /// <param name="query">GET parameters in URL form</param>
        /// <returns></returns>
        public string queryApi(string query)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_API_URI + query);
            //req.Headers.Add("Accept", "application/json");
            req.Accept = "application/json";
            //req.Method = "GET";
            req.ContentType = "application/json; charset=utf-8";
            WebResponse httpResp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(httpResp.GetResponseStream());

            String resp = sr.ReadToEnd();
            return resp;
        }

        public List<ApiDataTypes.FlightPlanSummary> searchFlightPlans(string startIcao, string destIcao)
        {
            string q = "search/plans?fromICAO=" + startIcao + "&toICAO=" + destIcao + "&limit=10";
            string r = queryApi(q);

            List<ApiDataTypes.FlightPlanSummary> flightPlans = ApiDataTypes.deserializeJsonArray<ApiDataTypes.FlightPlanSummary>(r);
            //ApiDataTypes.FlightPlanSummary mostPopularFpl = (from d in flightPlans orderby d.popularity descending select d).FirstOrDefault();
            return flightPlans;
        }

        public ApiDataTypes.FlightPlanDetails getPlan(string planId)
        {
            string q = "plan/" + planId;
            string r = queryApi(q);
            ApiDataTypes.FlightPlanDetails d = ApiDataTypes.deserializeJson<ApiDataTypes.FlightPlanDetails>(r);
            return d;
        }

    }
}
