using System.Net;
using System.Text;
using System.Text.Json;
using HttpServer;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var server = new Server();
            server.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Server finished");
        }
    }
}

/*using System;
   using System.Net;
   using System.Text;
   using System.IO;
   using System.Web;
   using Npgsql; 
   
   class Program
   {
   static void Main(string[] args)
   {
   string htmlFilePath = @"/Users/kirillshelokov/Desktop/Семестровка ОРИС/HttpServer/HttpServer/bin/Debug/net7.0/static/relationshipPage/relationship.html"; 
   
   HttpListener server = new HttpListener();
   server.Prefixes.Add("http://localhost:8080/");
   
   try
   {
   server.Start(); 
   
   Console.WriteLine("Сервер запущен. Ожидание запросов...");
   
   while (true)
   {
   HttpListenerContext context = server.GetContext();
   
   string responseHtml = File.ReadAllText(htmlFilePath); 
   string databaseData = LoadAdvDataFromDatabase(); 
   responseHtml = responseHtml.Replace("{advplaceholder}", databaseData); 
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
   string connectionString = "Host=localhost;Port=5432;Database=sem1;Username=user_A;Password=12345";
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
   
   static string LoadAdvDataFromDatabase()
   {
   string connectionString = "Host=localhost;Port=5432;Database=sem1;Username=user_A;Password=12345";
   
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
   }*/