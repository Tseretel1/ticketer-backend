using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using QRCoder;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;

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
        public async Task TicketBought(string email, int boughtCount, Ticket ticket)
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
                       .image-container {{
                           display: flex;
                           justify-content: center;
                           align-items: center;
                           width: 100%;
                           height: 200px;
                           overflow: hidden;
                           border-radius: 10px;
                       }}
                       .ticket-image {{
                           width :100%;
                           height:100%;
                           object-fit:cover;
                       }}
                       .ticket-title {{
                           padding:20px 0px;
                       }}  
                       .ticket-count {{
                           padding:20px 0px;
                       }}
                   </style>
               </head>
               <body>                   
                    <div class='ticket-title'>გილოცავთ, თქვენ წარმატებით შეიძინეთ ბილეთი : {ticket.Title}.  </div>
                    <div class='image-container'>
                         <img src='{ticket.Photo}' class='ticket-image'>
                    </div>
                    <div class='ticket-count'>ბილეთების როდენობა : {boughtCount}</div>
               </body>
           </html>";

                string subject = $"{ticket.Title}";
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
