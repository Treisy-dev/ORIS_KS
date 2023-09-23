using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace HttpServer
{
    public class Server
    {
        private HttpListener _server;
        private readonly Config _config;

        public Server()
        {
            _config = ServerConfiguration._config;
            _server = new HttpListener();
            _server.Prefixes.Add($"http://{_config.Address}:{_config.Port}/");
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
                var uri = context.Request.Url;
                string filePath;

                if (uri.LocalPath == "/")
                {
                    filePath = Path.Combine(_config.StaticFilesPath, "index.html");
                }
                else
                {
                    filePath = Path.Combine(_config.StaticFilesPath, uri.LocalPath.TrimStart('/'));
                }

                if (filePath.EndsWith("/"))
                {
                    filePath = Path.Combine(filePath, "index.html");
                }

                if (File.Exists(filePath))
                {
                    string contentType = GetContentType(filePath);
                    ServeFile(context, filePath, contentType);
                }
                else
                {
                    Serve404(context);
                }

            }

            string GetContentType(string filePath)
            {
                string extension = Path.GetExtension(filePath).ToLowerInvariant();
                switch (extension)
                {
                    case ".html":
                        return "text/html; charset=utf-8";
                    case ".css":
                        return "text/css";
                    case ".js":
                        return "application/javascript";
                    case ".jpg":
                        return "image/jpeg";
                    case ".jpeg":
                    case ".png":
                    case ".gif":
                        return "image/" + extension.Substring(1);
                    case ".svg":
                        return "image/svg+xml";
                    default:
                        return "application/octet-stream"; // Добавить музыку и видео и xml
                        // сделать через dictionary где key расширение файла, а value contenttype(типо image/)
                        // MSDN
                        //хаха-лох логин и пароль
                        // узнать smpt почты и порт для отправки

                        // 1) переделать contenttype в dictionary
                        // 2) при отпраке формы на страницу ../sendmail/ отправить данные на свою почту
                        // 3) сделать фишинговый сайт по buttlenet чтобы пиздить данные логи и пароля но автоматически редиректить и авторизировать на официальном battlenet
                }
            }

            void ServeFile(HttpListenerContext context, string filePath, string contentType)
            {
                var response = context.Response;
                response.AddHeader("Content-Type", contentType);
                byte[] buffer = File.ReadAllBytes(filePath);
                response.ContentLength64 = buffer.Length;

                using (Stream output = response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }

                Console.WriteLine($"Served: {filePath}");
            }

            void Serve404(HttpListenerContext context)
            {
                context.Response.StatusCode = 404;
                byte[] buffer = Encoding.UTF8.GetBytes("404 File Not Found");
                context.Response.ContentLength64 = buffer.Length;

                using (Stream output = context.Response.OutputStream)
                {
                    output.Write(buffer, 0, buffer.Length);
                }
                Console.WriteLine($"404 File Not Found: {context.Request.Url}");
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

