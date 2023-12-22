using HttpServer.ORM;

namespace HttpServer.templator;

public class RelationshipTemplator
{
    private string url;

    public RelationshipTemplator(string url)
    {
        this.url = url;
    }
    public string getResponseHtml()
    {
        var advData = new AdvData();
        var favoritePagesTableData = new FavoritePagesTableData();
        var relationshipBottomTableData = new RelationshipBottomTableData();
        string responseHtml = File.ReadAllText(url);
        responseHtml = responseHtml.Replace("{advplaceholder}", advData.GetAdv());
        responseHtml = responseHtml.Replace("{favoritePages}", favoritePagesTableData.GetFavoritePages());
        responseHtml = responseHtml.Replace("{bottomTable}", relationshipBottomTableData.getRelationshipBottomTableData());
        return responseHtml;
    }
}