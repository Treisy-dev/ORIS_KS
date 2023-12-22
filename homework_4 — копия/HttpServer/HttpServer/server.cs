using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

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
                else if (uri.LocalPath == "/sendMail")
                {
                    HandleFormSubmission(context);
                    return;
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



        private void HandleFormSubmission(HttpListenerContext context)
        {
            if (context.Request.HttpMethod == "POST")
            {
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    var data = reader.ReadToEnd();
                    var formData = HttpUtility.ParseQueryString(data);

                    var login = formData["login"];  //почта 
                    var password = formData["password"];

                    string emailTo = login;
                    string emailFrom = "postgetpost@yandex.ru";
                    string emailPassword = "nylnmfswmeubcprd";

                    string subject = "Сообщение от Щёлоков Кирилл";
                    string body = "ваш логин: " + login +
                                 "ваш \nпароль: " + password;

                    Console.WriteLine($"Login: {login}");
                    Console.WriteLine($"Password: {password}");

                    MailMessage message = new MailMessage(emailFrom, emailTo, subject, body);
                    message.Attachments.Add(new Attachment("homework.zip"));
                    SmtpClient smtp = new SmtpClient("smtp.yandex.ru", 25);
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential(emailFrom, emailPassword);
                    smtp.Send(message);
                }
            }

            var response = context.Response;
            string responseText = "<html><body><p>Запрос обработан, вы можете вернуться назад</p></body></html>";
            response.ContentType = "text/html; charset=utf-8";
            byte[] buffer = Encoding.UTF8.GetBytes(responseText);
            response.ContentLength64 = buffer.Length;

            using (Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
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

