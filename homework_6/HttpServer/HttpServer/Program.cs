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
            var server = new Server();
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


