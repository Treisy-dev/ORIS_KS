using HttpServer.ORM;

namespace HttpServer.templator;

public class AdminTemplator
{
    private string url;

    public AdminTemplator(string url)
    {
        this.url = url;
    }
    public string getResponseHtml()
    {
        var advData = new AdvData();
        var favoritePagesTableData = new FavoritePagesTableData();
        string responseHtml = File.ReadAllText(url);
        responseHtml = responseHtml.Replace("{advTable}", advData.GetAllAdv());
        responseHtml = responseHtml.Replace("{famousPagesTable}", favoritePagesTableData.GetAllFavoritePages());
        return responseHtml;
    }
}