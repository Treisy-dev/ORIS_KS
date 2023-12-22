using System.Net;
using HttpServer.Handlers;

namespace HttpServer
{
    public class Server
    {
        
        private HttpListener _server;
        private readonly Config _config;

        private Handler _staticFilesHandler;

        public Server()
        {
            _config = ServerConfiguration._config;
            _server = new HttpListener();
            _server.Prefixes.Add($"http://{_config.Address}:{_config.Port}/");
            _staticFilesHandler = new StaticFilesHandler();
        }

        public void Start()
        { 
            if (!Directory.Exists(_config.StaticFilesPath))
            {
                Directory.CreateDirectory(_config.StaticFilesPath);
            }

            Console.WriteLine("Сервер запущен. Ожидание запросов...");
            _server.Start();

            while (true)
            {
                var context = _server.GetContextAsync();
                ProcessRequest(context.Result);
            }

            void ProcessRequest(HttpListenerContext context)
            {
                _staticFilesHandler.HandleRequest(context);
            }
        }


        public void Stop()
        {
            _server.Stop();
        }

        public async Task<HttpListenerContext> GetContextAsync()  
        {
            return await _server.GetContextAsync();
        }
    }
}