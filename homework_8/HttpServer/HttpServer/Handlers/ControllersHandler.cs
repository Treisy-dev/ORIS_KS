using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Web;
using HttpServer.Attributes;

namespace HttpServer.Handlers
{
    public class ControllersHandler : Handler
    { 
        public override void HandleRequest(HttpListenerContext _httpContext)
        {
            var uriSegments = _httpContext.Request.Url.Segments;  
            string[] strParams = uriSegments
                .Skip(1)
                .Select(s => s.Replace("/", ""))  //../accounts/add
                .ToArray();


            var controllerName = strParams[^2];
            var methodName = strParams[^1];
            var login = "";
            var password = "";

            using (var reader = new StreamReader(_httpContext.Request.InputStream, _httpContext.Request.ContentEncoding))
            {
                var data = reader.ReadToEnd();
                var formData = HttpUtility.ParseQueryString(data);

                login = formData["login"];  //почта 
                password = formData["password"];
            }


            // string.Equals(c.Name, controllerName, StringComparison.CurrentCultureIgnoreCase)

            var assembly = Assembly.GetExecutingAssembly();
            var controller = assembly
                .GetTypes()
                .Where(t => Attribute.IsDefined(t, typeof(HttpController)))
                .FirstOrDefault(c =>
                    ((HttpController)Attribute.GetCustomAttribute(c, typeof(HttpController))!).name.Equals(controllerName));


            // if (controller == null) return false;

            var method = controller
                .GetMethods()
                .FirstOrDefault(x => x.GetCustomAttributes(true)
                .Any(attr => attr.GetType().Name.Equals($"Http{_httpContext.Request.HttpMethod}Attribute",
                StringComparison.OrdinalIgnoreCase) && ((HttpMethodAttribute)attr).actionName.Equals(methodName, StringComparison.OrdinalIgnoreCase)));

            // if (method == null) return false;

            

            string[] strParams1 = new string[] { login, password };

            object[] queryParams = method
                .GetParameters()
                .Select((p, i) => Convert.ChangeType(strParams1[i], p.ParameterType))
                .ToArray();

            var result = method.Invoke(Activator.CreateInstance(controller), queryParams);

            _httpContext.Response.ContentType = "text/html";
            
            byte[] buffer = Encoding.UTF8.GetBytes((String)result);

            _httpContext.Response.ContentLength64 = buffer.Length;
            using Stream output = _httpContext.Response.OutputStream;

            output.Write(buffer, 0, buffer.Length);
            output.Flush();

        }
    }
}

