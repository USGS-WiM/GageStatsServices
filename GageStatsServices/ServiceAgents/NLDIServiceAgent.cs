using System;
using System.Net.Http;
using GageStatsAgent.Resources;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GageStatsAgent.ServiceAgents
{
    public class NLDIServiceAgent
    {
        #region Properties
        public NLDISettings NLDIsettings { get; set; }
        private NavigationSettings Navigationsettings { get; set; }
        private object NLDIstations;

        #endregion
        #region Constructors
        public NLDIServiceAgent(NLDISettings settings, NavigationSettings navsettings)

        {
            this.NLDIsettings = settings;
            this.Navigationsettings = navsettings;
        }
        #endregion
        #region Methods
        public async Task<bool> ReadNLDIAsync(double lat, double lon, double distance)
        {
            string result = "";
            string msg;
            dynamic obj = null;

            try
            {
                var sa = new NavigationServiceAgent(this.Navigationsettings);
                var isOk = await sa.ReadNavigationAsync(lat, lon);

                if (!isOk) throw new Exception("Failed to retrieve data from Navigation Services");

                object[] args = { sa.getComid(), distance };
                string urlString = String.Format(getURI(serviceType.e_downstream), args);

                using (HttpClient conn = new HttpClient())                   
                {
                    conn.BaseAddress = new Uri(NLDIsettings.baseurl);

                    var reply = await conn.GetAsync(urlString);
                    result = reply.Content.ReadAsStringAsync().Result;
                    if (result != "")
                    {
                        obj = JsonConvert.DeserializeObject<dynamic>(result);
                    }
                }

                urlString = String.Format(getURI(serviceType.e_upstream), args);

                using (HttpClient conn = new HttpClient())
                {
                    conn.BaseAddress = new Uri(NLDIsettings.baseurl);

                    var reply = await conn.GetAsync(urlString);
                    result = reply.Content.ReadAsStringAsync().Result;
                    if (result != "")
                    {
                        if (obj)
                        {
                            obj = obj.add(JsonConvert.DeserializeObject<dynamic>(result));
                        }
                    }                    
                    this.NLDIstations = obj;
                }
                if (isDynamicError(result, out msg)) throw new Exception(msg);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }//end PostFeatures
        public object getStations()
        {
            if(this.NLDIstations != null)
            {
                return this.NLDIstations;
            }
            else
            {
                return "No stations located within search distance.";
            }            
        }

        #endregion
        #region Helper Methods

        private String getURI(serviceType sType)
        {
            string uri = string.Empty;
            
            switch(sType)
            {
                case serviceType.e_downstream:
                    uri = NLDIsettings.resources["downstreamQuery"];
                    break;
                case serviceType.e_upstream:
                    uri = NLDIsettings.resources["upstreamQuery"];
                    break;

            }
            return uri;
        }//end getURL
        private Boolean isDynamicError(dynamic obj, out string msg)
        {
            msg = string.Empty;
            try
            {
                var error = obj.error;
                if (error == null) throw new Exception();
                msg = error.message;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        //select generic storm event, All Cases and 50% probability
        private async Task<double> getComid(double lat, double lon)
        {
            string urlString = "/NavigationServices/attributes?x=" + lon + "&y=" + lat;
            double comid;
            try
            {
                using (HttpClient conn = new HttpClient())
                {
                    conn.BaseAddress = new Uri("https://test.streamstats.usgs.gov");
                    var reply = await conn.GetAsync(urlString, default(System.Threading.CancellationToken));
                    comid = 1; //reply.COMID;
                }
                return comid;
            }
            catch(Exception)
            {
                return 0;
            }
        }
        #endregion
        #region Enumerations
        public enum serviceType
        {
            e_downstream,
            e_upstream
        }
        #endregion
    }
}
