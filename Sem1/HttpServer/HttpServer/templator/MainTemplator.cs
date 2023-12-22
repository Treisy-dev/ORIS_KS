using HttpServer.ORM;
namespace HttpServer.templator;

public class MainTemplator
{
    private string url;

    public MainTemplator(string url)
    {
        this.url = url;
    }
    public string getResponseHtml()
    {
        var advData = new AdvData();
        var firstTableData = new СategoriesTableData();
        var mainBottomTableData = new MainBottomTableData();
        string responseHtml = File.ReadAllText(url);
        responseHtml = responseHtml.Replace("{advplaceholder}", advData.GetAdv());
        responseHtml = responseHtml.Replace("{firstTable}", firstTableData.getСategoriesTableData());
        responseHtml = responseHtml.Replace("{bottomTable}", mainBottomTableData.getMainBottomTableData());
        return responseHtml;
    }

}