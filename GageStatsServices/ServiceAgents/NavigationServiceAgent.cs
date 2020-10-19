using System;
using System.Net.Http;
using GageStatsAgent.Resources;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace GageStatsAgent.ServiceAgents
{
    public class NavigationServiceAgent
    {
        #region Properties
        public NavigationSettings settings { get; set; }
        public string comid;

        #endregion
        #region Constructors
        public NavigationServiceAgent(NavigationSettings settings)
        {
            this.settings = settings;
        }
        #endregion
        #region Methods
        public async Task<bool> ReadNavigationAsync(double lat, double lon)
        {
            string result = "";
            string msg;

            try
            {
                object[] args = { lat, lon };
                string urlString = String.Format(getURI(), args);
                using (HttpClient conn = new HttpClient())                   
                {
                    conn.BaseAddress = new Uri(settings.baseurl);

                    var reply = await conn.GetAsync(urlString);

                    result = reply.Content.ReadAsStringAsync().Result;
                    dynamic obj = JsonConvert.DeserializeObject<dynamic>(result);
                    this.comid = obj[0].COMID;
                }

                if (isDynamicError(result, out msg)) throw new Exception(msg);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }//end PostFeatures
        public string getComid()
        {
            return this.comid;
        }
        #endregion
        #region Helper Methods

        private String getURI()
        {
            string uri = string.Empty;
            
            uri = settings.resources["navigationQuery"];

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
    }
}
