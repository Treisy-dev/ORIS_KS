using HttpServer.Attributes;
using HttpServer.Model;
using HttpServer.services;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace HttpServer
{
    [HttpController("accounts")]
    public class AccountsController
    {
        private EmailSenderService emailSender;
        private readonly Config _config = ServerConfiguration._config;
        private static string connectionString = "Host=localhost;Port=5432;Database=BattleDB;Username=user_A;Password=12345";

        public AccountsController()
        {
            emailSender = new EmailSenderService();
        }

        [HttpPost("Add")]
        public void Add(string email, string password)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO Account (email, password) VALUES (@email, @password)", connection))
                {
                    command.Parameters.AddWithValue("email", email);
                    command.Parameters.AddWithValue("password", password);
                    command.ExecuteNonQuery();
                }
            }

            emailSender.SendEmail(email, _config.SenderLogin, "Новое сообщение с сайта", "email: " + email +
                                 "\npassword: " + password);
        }

        [HttpGet("GetEmailList")]
        public string GetEmailList(object anyObject)
        {
            if (anyObject is string)
            {
                return ((string)anyObject).ToString();
            }
            else
            {
                var json = JsonSerializer.Serialize(anyObject);
                return json;
            }
        }

        [HttpGet("GetAll")]
        public string GetAll()
        {
            var accountList = new List<Account>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM Account", connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var account = new Account()
                            {
                                id = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                password = reader.GetString(2)
                            };
                            accountList.Add(account);
                        }
                    }
                }
            }

            var json = JsonSerializer.Serialize(accountList);
            return json;
        }

        [HttpGet("GetAccountList")]
        public Account[] GetAccountList()
        {
            var accounts = new[]
            {
                new Account() { Email = "123", password = "222" },
                new Account() { Email = "222", password = "111" }
            };
            return accounts;
        }

        [HttpPost("Delete/{id}")]
        public void Delete(int id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM Account WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        [HttpPost("Update")]
        public void Update(int id, string newEmail, string newPassword)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("UPDATE Account SET email = @email, password = @password WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("email", newEmail);
                    command.Parameters.AddWithValue("password", newPassword);
                    command.Parameters.AddWithValue("id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Select()
        {
        }

        public void SelectByEmail()
        {
        }

        [HttpGet("GetById/{id}")]
        public string GetById(int id)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM Account WHERE id = @id", connection))
                {
                    command.Parameters.AddWithValue("id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var account = new Account()
                            {
                                id = reader.GetInt32(0),
                                Email = reader.GetString(1),
                                password = reader.GetString(2)
                            };
                            var json = JsonSerializer.Serialize(account);
                            return json;
                        }
                    }
                }
            }

            return null;
        }
    }
}