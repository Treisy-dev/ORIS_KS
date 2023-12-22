using System.Net;
using System.Web;

namespace HttpServer.Auth;

public class Login
{
    private HttpListenerRequest request;
    public Login(HttpListenerRequest request)
    {
        this.request = request;
    }
    
    public bool checkLogin()
    {
        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
        {
            var data = reader.ReadToEnd();
            var formData = HttpUtility.ParseQueryString(data);

            var login = formData["username"];
            var password = formData["password"];

            if (login == "admin" && password == "admin")
            {
                if (login == "admin" && password == "admin")
                {
                    return true;
                }
            }
        }

        return false;
    }
}