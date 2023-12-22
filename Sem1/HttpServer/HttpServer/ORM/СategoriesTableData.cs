using System.Text;
using Npgsql;
namespace HttpServer.ORM;

public class СategoriesTableData
{

    public string getСategoriesTableData()
    {
        StringBuilder htmlBuilder = new StringBuilder();
        htmlBuilder.Append(LoadFirstTableLineDataFromDatabase());
        htmlBuilder.Append(LoadSecondTableLineDataFromDatabase());
        return htmlBuilder.ToString();
    }
    
    private string LoadFirstTableLineDataFromDatabase()
        {
            string connectionString = ServerConfiguration._config.ConnectionSctring;
   
            string query = "SELECT * FROM first_table LIMIT 6";
   
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open(); 
   
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        StringBuilder htmlBuilder = new StringBuilder();
                        htmlBuilder.Append("<tr>");
   
                        while (reader.Read())
                        {
                            string image_src = reader.GetString(0);
                            string link = reader.GetString(1);
                            string alter = reader.GetString(2);
                            string name = reader.GetString(3);
   
                            htmlBuilder.Append("<td width=\"16%\" valign=\"top\"><div class=\"portal_sliderlink_4\">");
                            htmlBuilder.Append($"<a href=\"{link}\" title=\"{name}\"><img alt=\"{alter}\" src=\"{image_src}\" width=\"80\" height=\"80\"></a>");
                            htmlBuilder.Append("<br>");
                            htmlBuilder.Append($"<a href=\"{link}\" title=\"{name}\">{name}</a>");
                            htmlBuilder.Append("</div></td>");
                        }
                        htmlBuilder.Append("</tr><tr><td colspan=\"6\" style=\"height: 10px;\"></td></tr>");
                        return htmlBuilder.ToString();
                    }
                }
            }
        }
        
        private string LoadSecondTableLineDataFromDatabase()
        {
            string connectionString = ServerConfiguration._config.ConnectionSctring;
   
            string query = "SELECT * FROM first_table LIMIT 6 OFFSET 6;";
   
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open(); 
   
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        StringBuilder htmlBuilder = new StringBuilder();
                        htmlBuilder.Append("<tr>");
   
                        while (reader.Read())
                        {
                            string image_src = reader.GetString(0);
                            string link = reader.GetString(1);
                            string alter = reader.GetString(2);
                            string name = reader.GetString(3);
   
                            htmlBuilder.Append("<td width=\"16%\" valign=\"top\"><div class=\"portal_sliderlink_4\">");
                            htmlBuilder.Append($"<a href=\"{link}\" title=\"{name}\"><img alt=\"{alter}\" src=\"{image_src}\" width=\"80\" height=\"80\"></a>");
                            htmlBuilder.Append("<br>");
                            htmlBuilder.Append($"<a href=\"{link}\" title=\"{name}\">{name}</a>");
                            htmlBuilder.Append("</div></td>");
                        }
                        htmlBuilder.Append("</tr>");
                        return htmlBuilder.ToString();
                    }
                }
            }
        }
}