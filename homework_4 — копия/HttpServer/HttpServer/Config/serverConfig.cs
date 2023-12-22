using System;
using System.Text.Json;

namespace HttpServer
{
    public static class ServerConfiguration
    {
        public static Config _config { get; }

        static ServerConfiguration()
        {
            try
            {
                using (var file = File.OpenRead(@"appsettings.json"))
                {
                    _config = JsonSerializer.Deserialize<Config>(file) ?? throw new Exception();
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

    public class Config
    {
        public string Address { get; set; }
        public int Port { get; set; }
        public string StaticFilesPath { get; set; }
    }
}

