using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using WebStore.Logger;

namespace WebStore
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(host => host
                   .UseStartup<Startup>()
                   //.ConfigureLogging((host, log) => log
                   //    .ClearProviders()
                   //    .AddDebug()
                   //    .AddConsole(opt => opt.IncludeScopes = true)
                   //    .AddEventLog()
                   //    .AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.Warning))
                )
               .ConfigureLogging(log => log.AddLog4Net())
               .UseSerilog((host, log) => log.ReadFrom.Configuration(host.Configuration)
                   .MinimumLevel.Debug()
                   .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                   .Enrich.FromLogContext()
                   .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}]{SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}"
                        )
                   .WriteTo.RollingFile($@".\Logs\WebStore[{DateTime.Now:yyyy-MM-ddTHH-mm-ss}].log")
                   .WriteTo.File(new JsonFormatter(",", true), $@".\Logs\WebStore[{DateTime.Now:yyyy-MM-ddTHH-mm-ss}].log.json")
                   .WriteTo.Seq("http://localhost:5341")
                )
            ;
    }
}
