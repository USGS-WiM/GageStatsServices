using System;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using GageStatsServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GageStatsDB.Resources;
using GageStatsAgent;
using GageStatsServices.Controllers;

namespace GageStatsGeometry.Test
{
    [TestClass]
    public class GeometryTest
    {

        [TestMethod]
        public void GeometryCheck()
        {
            try
            {
                //Station stationtest = new Station();
                //public IGageStatsAgent agent { get; set; }
                //public StationsController(IGageStatsAgent agent) : base()
                //{
                //    this.agent = agent;
                //}
            }
            catch (Exception ex)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(false, ex.Message);
            }
        }
    }
}
