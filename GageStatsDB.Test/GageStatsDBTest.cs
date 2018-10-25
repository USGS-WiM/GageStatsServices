using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace GageStatsDB.Test
{
    [TestClass]
    public class GageStatsDBTest
    {
        private string connectionstring = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build().GetConnectionString("Connection");

        [TestMethod]
        public void ConnectionTest()
        {
            using (GageStatsDBContext context = new GageStatsDBContext(new DbContextOptionsBuilder<GageStatsDBContext>().UseNpgsql(this.connectionstring, x => x.UseNetTopologySuite()).Options))
            {
                try
                {
                    if (!(context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()) throw new Exception("db does ont exist");
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(false, ex.Message);
                }
            }
        }
        [TestMethod]
        public void QueryTest()
        {
            using (GageStatsDBContext context = new GageStatsDBContext(new DbContextOptionsBuilder<GageStatsDBContext>().UseNpgsql(this.connectionstring, x => x.UseNetTopologySuite()).Options))
            {
                try
                {
                    var testQuery = context.ErrorTypes.ToList();
                    Assert.IsNotNull(testQuery, testQuery.Count.ToString());
                }
                catch (Exception ex)
                {
                    Assert.IsTrue(false, ex.Message);
                }
                finally
                {
                }

            }
        }
    }
}
