using System.Text;
using Npgsql;

namespace HttpServer.ORM;

public class MainBottomTableData
{
    public string getMainBottomTableData()
    {
        StringBuilder htmlBuilder = new StringBuilder();
        htmlBuilder.Append(LoadBottomTableFirstColumnDataFromDatabase());
        htmlBuilder.Append(LoadBottomTableSecondColumnDataFromDatabase());
        htmlBuilder.Append(LoadBottomTableThirdColumnDataFromDatabase());
        return htmlBuilder.ToString();
    }
    
    private string LoadBottomTableFirstColumnDataFromDatabase()
        {
            string connectionString = ServerConfiguration._config.ConnectionSctring;
   
            string query = "SELECT * FROM second_table LIMIT 2;";
   
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open(); 
   
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        StringBuilder htmlBuilder = new StringBuilder();
                        while (reader.Read())
                        {
                            string image_src = reader.GetString(0);
                            string link = reader.GetString(1);
                            string alter = reader.GetString(2);
                            string name = reader.GetString(3);
                            string sub_name = reader.GetString(4);
   
                            htmlBuilder.Append($"<div class=\"mcf-card-article__link \"><a href=\"{link}\" class=\" mcf-card mcf-card-article\">");
                            htmlBuilder.Append($"<img class=\"mcf-card-article__thumbnail\" src=\"{image_src}\" alt=\"{alter}\">");
                            htmlBuilder.Append($"<span class=\"mcf-card-article__wrapper has-thumbnail\"><span class=\"mcf-card-article__title\">{name}</span><span class=\"mcf-card-article__subtitle\">{sub_name}");
                            htmlBuilder.Append($"</span></span></a></div>");
                        }
                        htmlBuilder.Append("</div><div class=\"mcf-column\">");
                        return htmlBuilder.ToString();
                            
                    }
                }
            }
        }
        
        private string LoadBottomTableSecondColumnDataFromDatabase()
        {
            string connectionString = ServerConfiguration._config.ConnectionSctring;
   
            string query = "SELECT * FROM second_table LIMIT 3 OFFSET 2;";
   
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open(); 
   
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        StringBuilder htmlBuilder = new StringBuilder();
                        while (reader.Read())
                        {
                            string image_src = reader.GetString(0);
                            string link = reader.GetString(1);
                            string alter = reader.GetString(2);
                            string name = reader.GetString(3);
                            string sub_name = reader.GetString(4);
   
                            htmlBuilder.Append($"<div class=\"mcf-card-article__link \"><a href=\"{link}\" class=\" mcf-card mcf-card-article\">");
                            htmlBuilder.Append($"<img class=\"mcf-card-article__thumbnail\" src=\"{image_src}\" alt=\"{alter}\">");
                            htmlBuilder.Append($"<span class=\"mcf-card-article__wrapper has-thumbnail\"><span class=\"mcf-card-article__title\">{name}</span><span class=\"mcf-card-article__subtitle\">{sub_name}");
                            htmlBuilder.Append($"</span></span></a></div>");
                        }
                        htmlBuilder.Append("</div><div class=\"mcf-column\">");
                        return htmlBuilder.ToString();
                    }
                }
            }
        }
        
        private string LoadBottomTableThirdColumnDataFromDatabase()
        {
            string connectionString = ServerConfiguration._config.ConnectionSctring;
   
            string query = "SELECT * FROM second_table LIMIT 3 OFFSET 5;";
   
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open(); 
   
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        StringBuilder htmlBuilder = new StringBuilder();
                        while (reader.Read())
                        {
                            string image_src = reader.GetString(0);
                            string link = reader.GetString(1);
                            string alter = reader.GetString(2);
                            string name = reader.GetString(3);
                            string sub_name = reader.GetString(4);
   
                            htmlBuilder.Append($"<div class=\"mcf-card-article__link \"><a href=\"{link}\" class=\" mcf-card mcf-card-article\">");
                            htmlBuilder.Append($"<img class=\"mcf-card-article__thumbnail\" src=\"{image_src}\" alt=\"{alter}\">");
                            htmlBuilder.Append($"<span class=\"mcf-card-article__wrapper has-thumbnail\"><span class=\"mcf-card-article__title\">{name}</span><span class=\"mcf-card-article__subtitle\">{sub_name}");
                            htmlBuilder.Append($"</span></span></a></div>");
                        }

                        htmlBuilder.Append("</div>");
                        return htmlBuilder.ToString();
                    }
                }
            }
        }
}