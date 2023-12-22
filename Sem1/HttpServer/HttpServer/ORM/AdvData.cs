using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Web;
using Npgsql;
namespace HttpServer.ORM;

public class AdvData
{
    public string GetAdv()
    {
        string connectionString = ServerConfiguration._config.ConnectionSctring;
   
        string query = "SELECT * FROM adv";
   
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
                        string name = reader.GetString(0);
                        string link = reader.GetString(1);
   
                        htmlBuilder.Append("<li>");
                        htmlBuilder.Append($"<a href=\"{link}\" class=\"global-footer__link\"> {name} </a>");
                        htmlBuilder.Append("</li>");
                    }
                    return htmlBuilder.ToString();
                }
            }
        }
    }
    
    public string GetAllAdv()
    {
        string connectionString = ServerConfiguration._config.ConnectionSctring;
        string query = "SELECT * FROM adv";
   
        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open(); 
   
            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    StringBuilder htmlBuilder = new StringBuilder();
                    htmlBuilder.Append("<tr><th>Имя ресурса</th><th>ссылка на ресурс</th></tr>");

                    while (reader.Read())
                    {
                        string name = reader.GetString(0);
                        string link = reader.GetString(1);

                        htmlBuilder.Append("<tr>");
                        htmlBuilder.Append($"<td>{name}</td>");
                        htmlBuilder.Append($"<td>{link}</td>");
                        htmlBuilder.Append("</tr>");
                    }
                    return htmlBuilder.ToString();
                }
            }
        }
    }
    
    public bool addValue(NameValueCollection data)
    {
        string connectionString = ServerConfiguration._config.ConnectionSctring;
        string query = "INSERT INTO adv (name, link) VALUES (@name, @link)";

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", data["name"]);
                command.Parameters.AddWithValue("@link", data["link"]);

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
    
    public bool DeleteAdvByLink(NameValueCollection data)
    {
        string connectionString = ServerConfiguration._config.ConnectionSctring;
        string query = "DELETE FROM adv WHERE link = @deleteLink;";

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
}