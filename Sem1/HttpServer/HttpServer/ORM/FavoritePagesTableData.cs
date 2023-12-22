using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using Npgsql;

namespace HttpServer.ORM;

public class FavoritePagesTableData
{
    public string GetFavoritePages()
    {
        string connectionString = ServerConfiguration._config.ConnectionSctring;
   
        string query = "SELECT * FROM famous_pages_table";
   
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
                        string title = reader.GetString(2);
                        string name = reader.GetString(3);
   
                        htmlBuilder.Append("<li class=\"popular-pages__item\">");
                        htmlBuilder.Append($"<a href=\"{link}\" title=\"{title}\">");
                        htmlBuilder.Append($"<img class=\"popular-pages__image\" alt=\"{title}\" width=\"53\" height=\"53\" src=\"{image_src}\">");
                        htmlBuilder.Append($"<span>{name}</span></a></li>");
                    }
                    
                    return htmlBuilder.ToString();
                }
            }
        }
    }

    public string GetAllFavoritePages()
    {
        string connectionString = ServerConfiguration._config.ConnectionSctring;
   
        string query = "SELECT * FROM famous_pages_table";
   
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open(); 
   
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    StringBuilder htmlBuilder = new StringBuilder();
                    htmlBuilder.Append("<tr><th>Путь в статике на картинку</th><th>ссылка на ресурс</th><th>Title для картинки</th><th>Имя страницы</th></tr>");

                    while (reader.Read())
                    {
                        string image_src = reader.GetString(0);
                        string link = reader.GetString(1);
                        string title = reader.GetString(2);
                        string name = reader.GetString(3);

                        htmlBuilder.Append("<tr>");
                        htmlBuilder.Append($"<td>{image_src}</td>");
                        htmlBuilder.Append($"<td>{link}</td>");
                        htmlBuilder.Append($"<td>{title}</td>");
                        htmlBuilder.Append($"<td>{name}</td>");
                        htmlBuilder.Append("</tr>");
                    }
                    
                    return htmlBuilder.ToString();
                }
            }
        }
    }
    
    public bool DeleteFavoritePageByLink(NameValueCollection data)
    {
        string connectionString = ServerConfiguration._config.ConnectionSctring;
        string query = "DELETE FROM famous_pages_table WHERE link = @deleteLink;";

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@deleteLink", data["deleteLink"]);

                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (NpgsqlException ex)
                {
                    if (ex.SqlState == "23505") // Код ошибки для совпадения первичного ключа
                    {
                        Console.WriteLine("Ошибка: Данные с таким ключом уже существуют.");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при добавлении данных: " + ex.Message);
                    }
                    return false;
                }
            }
        }
    }
    
    public bool addValue(NameValueCollection data)
    {
        string connectionString = ServerConfiguration._config.ConnectionSctring;
        string query = "INSERT INTO famous_pages_table (image_src, link, title, name) VALUES (@imageSrc, @link, @title, @name)";

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@imageSrc", data["image-src"]);
                command.Parameters.AddWithValue("@link", data["link"]);
                command.Parameters.AddWithValue("@title", data["title"]);
                command.Parameters.AddWithValue("@name", data["name"]);

                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (NpgsqlException ex)
                {
                    if (ex.SqlState == "23505") // Код ошибки для совпадения первичного ключа
                    {
                        Console.WriteLine("Ошибка: Данные с таким ключом уже существуют.");
                    }
                    else
                    {
                        Console.WriteLine("Ошибка при добавлении данных: " + ex.Message);
                    }
                    return false;
                }
            }
        }
    }
}