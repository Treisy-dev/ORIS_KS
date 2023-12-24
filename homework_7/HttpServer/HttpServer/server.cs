using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using HttpServer.Handlers;

namespace HttpServer
{
    public class Server
    {
        
        private HttpListener _server;
        private readonly Config _config;

        private Handler _staticFilesHandler;
        private Handler _controllersHandler;

        // staticHandler -> ControlHandler 

        public Server()
        {
            _config = ServerConfiguration._config;
            _server = new HttpListener();
            _server.Prefixes.Add($"http://{_config.Address}:{_config.Port}/");
            _staticFilesHandler = new StaticFilesHandler();
            _controllersHandler = new ControllersHandler();
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
                _staticFilesHandler.Successor = _controllersHandler;
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

