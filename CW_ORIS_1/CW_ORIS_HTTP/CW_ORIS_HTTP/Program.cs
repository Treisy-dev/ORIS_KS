using System;
using System.Net;
using System.Text;
using System.IO;
using System.Web;
using Npgsql; 

class Program
{
    static void Main(string[] args)
    {
        string htmlFilePath = "index.html"; 

        HttpListener server = new HttpListener();
        server.Prefixes.Add("http://localhost:8080/");

        try
        {
            server.Start(); 

            Console.WriteLine("Сервер запущен. Ожидание запросов...");

            while (true)
            {
                HttpListenerContext context = server.GetContext(); 

                if (context.Request.HttpMethod == "POST")
                {
                    using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                    {
                        var data = reader.ReadToEnd();
                        var formData = HttpUtility.ParseQueryString(data);

                        var id = int.Parse(formData["id"]);
                        var brand = formData["brand"];
                        var model = formData["model"];
                        var year = DateTime.Parse(formData["year"]);
                        var color = formData["color"];
                        var mileage = int.Parse(formData["mileage"]);
                        var is_available = formData["is_available"];
                        var add_year = DateTime.Parse(formData["add_year"]);
                        var car_count = int.Parse(formData["car_count"]);
                        
                        bool is_available_bool = false;
                        
                        if (is_available == "on")
                        {
                            is_available_bool = true;
                        }
                        
                        InsertDataIntoDatabase(id, brand, model, year, color, mileage, is_available_bool, add_year, car_count );
                    }
                }
                
                string responseHtml = File.ReadAllText(htmlFilePath); 
                string databaseData = LoadDataFromDatabase(); 
                responseHtml = responseHtml.Replace("{databaseData}", databaseData); 
                byte[] buffer = Encoding.UTF8.GetBytes(responseHtml);

                context.Response.ContentType = "text/html";
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                context.Response.OutputStream.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Произошла ошибка: " + ex.Message);
        }
        finally
        {
            server.Stop();
        }
    }
    
    static void InsertDataIntoDatabase(int id, string brand, string model, DateTime year, string color, int mileage, bool is_available, DateTime add_year, int car_count)
    {
        string connectionString = "Host=localhost;Port=5432;Database=CW_ORIS_1;Username=user_A;Password=12345";
        string query = "INSERT INTO cars (car_id, brand, model, prod_year, color, mileage, is_available, add_year, car_count) VALUES (@id, @brand, @model, @year, @color, @mileage, @is_available, @add_year, @car_count)";

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@brand", brand);
                command.Parameters.AddWithValue("@model", model);
                command.Parameters.AddWithValue("@year", year);
                command.Parameters.AddWithValue("@color", color);
                command.Parameters.AddWithValue("@mileage", mileage);
                command.Parameters.AddWithValue("@is_available", is_available);
                command.Parameters.AddWithValue("@add_year", add_year);
                command.Parameters.AddWithValue("@car_count", car_count);

                command.ExecuteNonQuery();
            }
        }
    }

    static string LoadDataFromDatabase()
    {
        string connectionString = "Host=localhost;Port=5432;Database=CW_ORIS_1;Username=user_A;Password=12345";
        
        string query = "SELECT * FROM cars";

        using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
        {
            connection.Open(); 

            using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    StringBuilder htmlBuilder = new StringBuilder();
                    htmlBuilder.Append("<tr><th>ID</th><th>Марка</th><th>Модель</th><th>Год выпуска</th><th>Цвет</th><th>Пробег</th><th>Доступность</th><th>Год добавления в бд</th><th>Количество</th></tr>");

                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string brand = reader.GetString(1);
                        string model = reader.GetString(2);
                        DateTime year = reader.GetDateTime(3);
                        string color = reader.GetString(4);
                        int mileage = reader.GetInt32(5);
                        bool isAvailable = reader.GetBoolean(6);
                        DateTime add_year = reader.GetDateTime(7);
                        int car_count = reader.GetInt32(8);

                        htmlBuilder.Append("<tr>");
                        htmlBuilder.Append($"<td>{id}</td>");
                        htmlBuilder.Append($"<td>{brand}</td>");
                        htmlBuilder.Append($"<td>{model}</td>");
                        htmlBuilder.Append($"<td>{year.ToShortDateString()}</td>");
                        htmlBuilder.Append($"<td>{color}</td>");
                        htmlBuilder.Append($"<td>{mileage}</td>");
                        htmlBuilder.Append($"<td>{(isAvailable ? "Да" : "Нет")}</td>");
                        htmlBuilder.Append($"<td>{add_year.ToShortDateString()}</td>");
                        htmlBuilder.Append($"<td>{car_count}</td>");
                        htmlBuilder.Append("</tr>");
                    }

                    htmlBuilder.Append("</table>");

                    return htmlBuilder.ToString();
                }
            }
        }
    }
}