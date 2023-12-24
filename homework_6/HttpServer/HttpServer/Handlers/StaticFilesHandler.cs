using System;
using System.IO;
using System.Linq;
using System.Net;

namespace HttpServer.Handlers
{
    public class StaticFilesHandler : Handler
    {
        private readonly Config _config = ServerConfiguration._config;

        private string GetUrl(HttpListenerContext context)

        {
            var url = context.Request.Url?.AbsolutePath.TrimEnd('/');

            if (url == null) throw new ArgumentNullException(url);

            if (url.Split('.')[^1] == "html")
            {
                if (Directory.GetFiles($"{_config.StaticFilesPath}/{url.Split('/')[^2]}")
                        .FirstOrDefault(x => x.Split('/')[^1] == url.Split('/')[^1]) == null)
                    url = $"/{url.Split('/')[^2]}" + "/not_found_page.html";
            }

            else if (Directory.GetDirectories(_config.StaticFilesPath)
                         .FirstOrDefault(x => x.Split('/')[^1] == url.Split('/')[^1]) != null)
            {
                url += "/index.html";
            }

            url = "static/" + url;

            url = string.Join('/', url.Split('/').ToHashSet());
            return url;
        }

        private async void SendResponseAsync(HttpListenerResponse response, string url)
        {
            var buffer = await File.ReadAllBytesAsync($"{url}");

            response.ContentType = ContentTypeManager.GetContentType(url);
            response.ContentLength64 = buffer.Length;
            await using var output = response.OutputStream;

            await output.WriteAsync(buffer);
            await output.FlushAsync();
        }

        public override void HandleRequest(HttpListenerContext context)
        {
            var url = GetUrl(context);

            if (url.Split('/').LastOrDefault()!.Contains('.'))
            {
                SendResponseAsync(context.Response, url);
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }
    }
}

