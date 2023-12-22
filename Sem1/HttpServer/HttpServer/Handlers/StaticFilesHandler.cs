using System.Net;
using System.Text;
using System.Web;
using HttpServer.Auth;
using HttpServer.ORM;
using HttpServer.templator;

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

            url = "static" + url;

            url = string.Join('/', url.Split('/').ToHashSet());
            return url;
        }

        private async void SendResponseAsync(HttpListenerResponse response, string url, HttpListenerRequest request)
        {
            byte[] buffer;

            if (request.HttpMethod == "POST" && url.EndsWith("/authPage/index.html"))
            {
                var login = new Login(request);
                if (login.checkLogin())
                {
                    response.Redirect("http://127.0.0.1:2323/authPage/admin.html");
                }
            }
            
            if (url.EndsWith("/mainPage/index.html"))
            {
                var mainTemplator = new MainTemplator(url);
                buffer = Encoding.UTF8.GetBytes(mainTemplator.getResponseHtml());
            }
            else if (url.EndsWith("/relationshipPage/index.html"))
            {
                var relationshipTemplator = new RelationshipTemplator(url);
                buffer = Encoding.UTF8.GetBytes(relationshipTemplator.getResponseHtml());
            }
            else if (url.EndsWith("/authPage/admin.html"))
            {
                var adminTemplator = new AdminTemplator(url);
                if (request.HttpMethod == "POST")
                {
                    string requestBody;
                    using (StreamReader reader = new StreamReader(request.InputStream))
                    {
                        requestBody = reader.ReadToEnd();
                    }
                    var parameters = HttpUtility.ParseQueryString(requestBody);
                    string buttonName = parameters.ToString();
                    if (buttonName.EndsWith("famous-page="))
                    {
                        var favoritePagesTableData = new FavoritePagesTableData();
                        if (favoritePagesTableData.addValue(parameters))
                        {
                            response.Headers.Add("Refresh", "0;url=/authPage/admin.html");
                        }
                        else
                        {
                            response.Redirect("http://127.0.0.1:2323/authPage/dataError.html");
                        }
                    }
                    else if (buttonName.EndsWith("advertisement="))
                    {
                        var advData = new AdvData();
                        if (advData.addValue(parameters))
                        {
                            response.Headers.Add("Refresh", "0;url=/authPage/admin.html");
                        }
                        else
                        {
                            response.Redirect("http://127.0.0.1:2323/authPage/dataError.html");
                        }
                    }
                    else if (buttonName.EndsWith("famous-page-delete="))
                    {
                        var favoritePagesTableData = new FavoritePagesTableData();
                        if (favoritePagesTableData.DeleteFavoritePageByLink(parameters))
                        {
                            response.Headers.Add("Refresh", "0;url=/authPage/admin.html");
                        }
                        else
                        {
                            response.Redirect("http://127.0.0.1:2323/authPage/dataError.html");
                        }
                    }
                    else if (buttonName.EndsWith("advertisement-delete="))
                    {
                        var advData = new AdvData();
                        if (advData.DeleteAdvByLink(parameters))
                        {
                            response.Headers.Add("Refresh", "0;url=/authPage/admin.html");
                        }
                        else
                        {
                            response.Redirect("http://127.0.0.1:2323/authPage/dataError.html");
                        }
                    }
                }
                buffer = Encoding.UTF8.GetBytes(adminTemplator.getResponseHtml());
            }
            else
            {
                buffer = await File.ReadAllBytesAsync($"{url}");
            }
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
                SendResponseAsync(context.Response, url, context.Request);
            }
            else if (Successor != null)
            {
                Successor.HandleRequest(context);
            }
        }
    }
}