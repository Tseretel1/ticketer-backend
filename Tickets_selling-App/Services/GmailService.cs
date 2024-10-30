using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QRCoder;
using Tickets_selling_App.Interfaces;

namespace Tickets_selling_App.Services
{
    public class GmailService : GmailInterface
    {
        private readonly string _fromAddress;
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        public GmailService(IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            _fromAddress = emailSettings["From"];
            _smtpServer = emailSettings["SmtpServer"];
            _port = int.Parse(emailSettings["Port"]);
            _username = emailSettings["UserName"];
            _password = emailSettings["Password"];
        }

        public async Task TicketBought(string email)
        {
            try
            {
                using var client = new SmtpClient(_smtpServer, _port)
                {
                    Credentials = new NetworkCredential(_username, _password),
                    EnableSsl = true
                };

                var fromAddress = new MailAddress(_fromAddress, "Ticket.ge");
                var toAddress = new MailAddress(email);
                string message = "You Successfully bought ticket!";
                string subject = "You Bought Ticket";
                var mailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task Email_Validation(string email, int passcode)
        {
            try
            {
                using var client = new SmtpClient(_smtpServer, _port)
                {
                    Credentials = new NetworkCredential(_username, _password),
                    EnableSsl = true
                };

                var fromAddress = new MailAddress(_fromAddress, "Ticketer");
                var toAddress = new MailAddress(email);

                string message = $@"
                   <html>
                       <head>
                           <style>
                               body {{ font-family: Arial, sans-serif; }}
                               .passcode {{ color: blue; font-weight: bold; }}
                           </style>
                       </head>
                       <body>
                           <h1>Email Validation</h1>
                           <p>We have sent you this passcode to validate your email, please enter this code on our website:</p>
                           <p class='passcode'>{passcode}</p>
                       </body>
                   </html>";

                string subject = "Your Passcode";
                var mailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = message,
                    
                    IsBodyHtml = true, 
                };

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
