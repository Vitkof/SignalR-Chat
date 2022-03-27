using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Net;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IConfiguration config = GetConfiguration();

            BuildHost(config, args).Run();
            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        
        private static IWebHost BuildHost(IConfiguration config, string[] args)
        {
            var builder = WebHost.CreateDefaultBuilder(args)
                .ConfigureKestrel(options =>
                {
                    int port = config.GetValue("ListenPort", 8334);

                    options.Listen(IPAddress.Any, port, listenOptions =>
                    {
                        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
                    });
                })
                .UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(config);

            return builder.Build();
        }

        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
