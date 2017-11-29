using Destructurama;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.IO;
using System.Net.WebSockets;

namespace AutomaticSwitchTimer
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .WriteTo.File(new JsonFormatter(),"log.json", rollingInterval: RollingInterval.Day)
                .Enrich.FromLogContext()
                .CreateLogger();
            Log.Logger = log.ForContext<HomeAssistantConnection>()
                .ForContext("AppDomain", "AutomaticSwitchTimer");
            try
            {
                Console.WriteLine("Hello World!");
                var connection = new HomeAssistantConnection(new Uri(@"ws://192.168.1.3:8123/api/websocket"));
                var a = connection.Authenticate().Result;
            }
            catch(Exception e)
            {
                Log.Logger.ForContext<Program>().Fatal(e, "we died :-(");
                throw;
            }
            
        }
    }
}
