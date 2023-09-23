using System;
using System.Text.Json;

namespace HttpServer
{
    public class ServerConfiguration
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public string StaticFilesPath { get; set; }

        public static ServerConfiguration Load()
        {
            try
            {
                using (var file = File.OpenRead(@"appsettings.json"))
                {
                    return JsonSerializer.Deserialize<ServerConfiguration>(file) ?? throw new Exception();
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Config file not found: " + @"appsettings.json");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading server configuration from " + @"appsettings.json");
                throw;
            }

        }
    }
}

