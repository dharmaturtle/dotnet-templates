using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace TodoBackendTemplate.Web
{
    static class Logging
    {
        public static LoggerConfiguration Configure(this LoggerConfiguration c) =>
            c
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                // .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console();
    }
    static class Program
    {
        public static async Task<int> Main(string[] argv)
        {
            try
            {
                Log.Logger = new LoggerConfiguration().Configure().CreateLogger();
                var host = WebHost
                    .CreateDefaultBuilder(argv)
                    .UseSerilog()
                    .UseStartup<Startup>()
                    .Build();
                // Conceptually, these can run in parallel
                // in practice, you'll only very rarely have >1 store
                foreach (var ctx in host.Services.GetServices<EquinoxContext>())
                    await ctx.Connect();
                host.Run();
                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return 1;
            }
        }
    }
}