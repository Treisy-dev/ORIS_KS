using System.Net;
using System.Text;
using System.Text.Json;
using HttpServer;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var config = ServerConfiguration.Load();
            var server = new Server(config);
            server.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Server finished");
        }
    }
}


