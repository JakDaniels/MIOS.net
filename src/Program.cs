using MIOS.net.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.VisualBasic;

namespace MIOS.net
{
    class Program
    {
        public static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var buildConfig = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddCommandLine(args)
                .Build();
            
            bool help = args.Contains("--mioshelp");

            if(!help)
            {
                var app = CreateHostBuilder(args, buildConfig).Build();
                app.Run();
                return;
            }

            Console.WriteLine("MIOS.net - Multiple Instances of OpenSimulator");
            Console.WriteLine("Usage: MIOS.net [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("  --mioshelp\t\t\tShow this help message");
            Console.WriteLine("  --port\t\t\tPort to listen on (default 5000)");
            Console.WriteLine("  --loopbackOnly\t\tOnly listen on loopback (default true)");
            Console.WriteLine("  --debug\t\t\tEnable debug console in UI (default false)");
            Console.WriteLine("  --exit\t\t\tExit server when UI is closed (default true)");
            Console.WriteLine("  --useUi\t\t\tEnable UI (default true)");

        }

        public static IHostBuilder CreateHostBuilder(string[] args, IConfiguration buildConfig)
        {
            var host = Host.CreateDefaultBuilder(args);

            bool loopbackOnly = buildConfig.GetValue<bool>("loopbackOnly", true);
            int listenPort = buildConfig.GetValue<int>("port", 5000);

            host.ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureKestrel(serverOptions =>
                {
                    serverOptions.Listen(loopbackOnly?IPAddress.Loopback:IPAddress.Any, listenPort, listenOptions =>
                    {
                        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    });
                });
                webBuilder.UseStartup<Startup>();
            });

            return host;
        }

    }
}
