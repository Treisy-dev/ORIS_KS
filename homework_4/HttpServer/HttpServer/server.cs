using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace HttpServer
{
    public class Server
    {
        private static Dictionary<string, string> mimeTypeMapping  = new Dictionary<string, string>()
                {
                    { ".html", "text/html; charset=utf-8" },
                    { ".css", "text/css" },
                    { ".js", "application/javascript" },
                    { ".jpg", "image/jpeg" },
                    { ".jpeg", "image/jpeg" },
                    { ".png", "image/png" },
                    { ".gif", "image/gif" },
                    { ".svg", "image/svg+xml" },
                    { ".mp3", "audio/mpeg" },
                    { ".wav", "audio/wav"},
                    { ".mp4", "video/mp4"},
                    { ".avi", "video/x-msvideo" },
                    { ".xml", "application/xml"}
                };
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
                    string contentType;
                    if (!mimeTypeMapping.TryGetValue(Path.GetExtension(filePath).ToLowerInvariant(), out contentType))
                    {
                        Console.WriteLine("application/octet-stream");
                    }
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
                    case ".mp3":
                        return "audio/mpeg";
                    case ".wav":
                        return "audio/wav";
                    case ".mp4":
                        return "video/mp4";
                    case ".avi":
                        return "video/x-msvideo";
                    case ".xml":
                    default:
                        return "application/octet-stream";
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

