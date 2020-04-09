using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using YounderBFF;

namespace Younder_BFF
{
    /// <summary>
    /// Program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Entrypoint
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.ApplicationInsights(TelemetryConverter.Events, LogEventLevel.Information)
                .WriteTo.Console(LogEventLevel.Debug)
                .CreateLogger();

            Log.Logger.Information($"From {typeof(Program).GetTypeInfo().Assembly.GetName().Name}. Running the host now.."); // This will be picked up up by AI

            try
            {
                var host  = CreateHostBuilder(args)
                                .UseSerilog()
                                .Build();
                
                host.Run();
            }
            catch (System.Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                throw;
            }
        }

        /// <summary>
        /// Create Host Builder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((host, config) =>
                {
                    var env = host.HostingEnvironment;
                    var currentDirectoryPath = Directory.GetCurrentDirectory();
                    config.SetBasePath(env.ContentRootPath);
                    config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                    config.AddEnvironmentVariables();
                }
                )
                .ConfigureLogging(logging =>
                {
                    logging.AddSerilog();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
