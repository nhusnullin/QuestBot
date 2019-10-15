using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ApplicationInsights.Sinks.ApplicationInsights.TelemetryConverters;

namespace CoreBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //TelemetryConfiguration.Active.TelemetryChannel.DeveloperMode = true;
            TelemetryConfiguration.Active.InstrumentationKey = "2b5097d8-ec90-4e24-a412-0d5cb1429d8f";


            //var telemetryClient = new TelemetryClient()
            //{
            //    InstrumentationKey = "2b5097d8-ec90-4e24-a412-0d5cb1429d8f"
            //};
            //telemetryClient.TrackException(new ApplicationException("nhusnullin exception"));
            //telemetryClient.Flush();

            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.ApplicationInsights(TelemetryConfiguration.Active, new CustomConverter())
                .CreateLogger();

//            Log.Logger = new LoggerConfiguration()
//                .ReadFrom.Configuration(configuration)
//                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.AddDebug();
                    logging.AddConsole();
                })
                
                .UseStartup<Startup>()
                .UseSerilog();
        }
    }
    
    public class CustomConverter : TraceTelemetryConverter
    {
        // пока оставим как идею, может чуток попзже пригодится
        public override IEnumerable<ITelemetry> Convert(LogEvent logEvent, IFormatProvider formatProvider)
        {
            foreach (ITelemetry telemetry in base.Convert(logEvent, formatProvider))
            {


                if (logEvent.Level != LogEventLevel.Information)
                {
                    yield return telemetry;
                }

                yield return telemetry;
            }
        }
    }
}