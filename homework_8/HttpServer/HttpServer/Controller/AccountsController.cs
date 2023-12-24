using System;
using System.Net;
using System.Text;
using HttpServer.Attributes;
using HttpServer.services;
using HttpServer.Model;
using System.Text.Json;

namespace HttpServer
{
    //  ..battlenet/accounts/add

    [HttpController("accounts")]
    public class AccountsController
	{
        private EmailSenderService emailSender;
        private readonly Config _config = ServerConfiguration._config;
        private static List<Account> accountList = new List<Account>();



        public AccountsController()
		{
            emailSender = new EmailSenderService();
		}

        [HttpPost("Add")]
        public void Add(string login, string password)
        {
            var account = new Account() { id = new Random().Next(), Email = login, password = password };
            accountList.Add(account);
            emailSender.SendEmail(login, _config.SenderLogin, "Новое сообщение с сайта", "login: " + login +
                                 "\npassword: " + password);
        }

        [HttpGet("GetEmailList")]
        public string GetEmailList(object anyObject)
        {
            if (anyObject is String)
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
            var json = JsonSerializer.Serialize(accountList);
            return json;
        }

        [HttpGet("GetAccountList")]
        public Account[] GetAccountList()
        {
            var accounts = new[]
            {
                new Account() {Email = "123", password = "222"},
                new Account() {Email = "222", password = "111"}

            };
            return accounts;
        }

        [HttpPost("Delete/{id}")]
        public void Delete(int id)
        {
            var account = accountList.Find(a => a.id == id);
            if (account != null)
            {
                accountList.Remove(account);
            }
        }

        [HttpPost("Update")]
        public void Update(int id, string newEmail, string newPassword)
        {
            var account = accountList.Find(a => a.id == id);
            if (account != null)
            {
                account.Email = newEmail;
                account.password = newPassword;
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
            var account = accountList.Find(a => a.id == id);
            if (account != null)
            {
                var json = JsonSerializer.Serialize(account);
                return json;
            }
            return null;
        }

    }
}

