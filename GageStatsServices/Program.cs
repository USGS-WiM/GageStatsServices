using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace GageStatsServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new HostBuilder()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(serverOptions => serverOptions.AddServerHeader = false)
                .UseStartup<Startup>();
            })
            .Build();

            host.Run();
        }
    }
}
