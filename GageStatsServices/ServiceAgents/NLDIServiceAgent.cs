using System;
using System.Net.Http;
using GageStatsAgent.Resources;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            string result_dn = "";
            string result_up = "";
            string msg;
            dynamic stations_obj = null;

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
                    result_dn = reply.Content.ReadAsStringAsync().Result;
                }
                if (isDynamicError(result_dn, out msg)) throw new Exception(msg);
                urlString = String.Format(getURI(serviceType.e_upstream), args);

                using (HttpClient conn = new HttpClient())
                {
                    conn.BaseAddress = new Uri(NLDIsettings.baseurl);

                    var reply = await conn.GetAsync(urlString);
                    result_up = reply.Content.ReadAsStringAsync().Result;                   
                }

                if (result_dn != "" && result_up != "")
                {
                    stations_obj = JsonConvert.SerializeObject(new[]
                    {
                            JsonConvert.DeserializeObject(result_dn), JsonConvert.DeserializeObject(result_up)
                        });
                    this.NLDIstations = stations_obj;
                }
                else if (result_dn == "" && result_up != "")
                {
                    stations_obj = JsonConvert.DeserializeObject<dynamic>(result_up);
                }
                else if (result_dn != "" && result_up == "")
                {
                    stations_obj = JsonConvert.DeserializeObject<dynamic>(result_dn);
                }

                if (stations_obj != null)
                {
                    this.NLDIstations = stations_obj;
                }
                if (isDynamicError(result_up, out msg)) throw new Exception(msg);

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
                return new JObject();
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
