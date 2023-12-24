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


        public AccountsController()
		{
            emailSender = new EmailSenderService();
		}

        [HttpPost("Add")]
		public void Add(string login, string password)
            // кому, от кого, заголовок сообщения, и тело сообщения
		{
            emailSender.SendEmail(login,_config.SenderLogin, "Новое сообщение с сайта", "login: " + login +
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

        public void Delete()
        {

        }

        public void Update()
        {

        }

        public void Select()
        {

        }

        public void SelectByEmail()
        {

        }

    }
}

