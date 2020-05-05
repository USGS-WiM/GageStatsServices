using System;
using Xunit;
using FU_GageStatsDB;
using Microsoft.Extensions.Configuration;

namespace FU_GageStatsDB.Test
{
    public class ForceUpdateTest
    {
        IConfiguration Configuration { get; set; }
        public ForceUpdateTest()
        {
            // the type specified here is just so the secrets library can
            // find the UserSecretId we added in the csproj file
            var builder = new ConfigurationBuilder()
                .AddUserSecrets<ForceUpdateTest>();
            Configuration = builder.Build();
        }

        [Fact]
        public void Test1Async()
        {
            try
            {
                var username = Configuration["dbuser"];
                var password = Configuration["dbpassword"];

                var x = new ForceUpdate(username, password, @"C:\Users\kjacobsen\Documents\wim_projects\docs\ss\nss\SSDB\StreamStatsDB_2020-03-27.mdb");
                if (x.VerifyLists())
                {
                    x.Load();
                //    x.LoadSqlFiles(@"D:\WiM\GitHub\NSSServices\FU_NSSDB\SQL_files");
                }

            }
            catch (Exception ex)
            {
                Assert.False(true, ex.Message);
            }

        }
    }
}
