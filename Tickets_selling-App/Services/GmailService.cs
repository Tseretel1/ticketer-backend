using System;
using System.Net;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;
using QRCoder;
using Tickets_selling_App.Interfaces;
using static QRCoder.PayloadGenerator;

namespace Tickets_selling_App.Services
{
    public class GmailService : Gmail_Interface
    {
        public async Task SendEmailAsync(string email, string qrCodeData)
        {
            try
            {
                var client = new SmtpClient("live.smtp.mailtrap.io", 587)
                {
                    Credentials = new NetworkCredential("api", "c0a4651a8cd800b259893fd94e8d1856"),
                    EnableSsl = true
                };

                var fromAddress = new MailAddress("mailtrap@demomailtrap.com", "Ticket.ge");
                var toAddress = new MailAddress(email);
                string message = "You Successfully bought ticket!";
                string subject = "You Bought Ticket";
                var mailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false,
                };
                byte[] qrCodeBytes = QrGenerator(qrCodeData);
                using (var qrStream = new MemoryStream(qrCodeBytes))
                {
                    mailMessage.Attachments.Add(new Attachment(qrStream, "qrcode.png", "image/png"));
                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public byte[] QrGenerator(string data)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                return qrCode.GetGraphic(50);
            }
        }

        public async Task Password_Restoration(string mail, int Passcode)
        {
            try
            {
                var client = new SmtpClient("live.smtp.mailtrap.io", 587)
                {
                    Credentials = new NetworkCredential("api", "c0a4651a8cd800b259893fd94e8d1856"),
                    EnableSsl = true
                };
                var fromAddress = new MailAddress("mailtrap@demomailtrap.com", "Ticket.ge");
                var toAddress = new MailAddress(mail);
                string message = $"{Passcode}";
                string subject = "Your Passcode";
                var mailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false,
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
                var client = new SmtpClient("live.smtp.mailtrap.io", 587)
                {
                    Credentials = new NetworkCredential("api", "c0a4651a8cd800b259893fd94e8d1856"),
                    EnableSsl = true
                };
                var fromAddress = new MailAddress("mailtrap@demomailtrap.com", "Ticket.ge");
                var toAddress = new MailAddress(email);
                string message = $" We have sent you this passcode to validate your email, please enter this code in our website " +
                    $"                                                                                                                                                                 " +
                    $"  {passcode}   ";
                string subject = "Your Passcode";
                var mailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false,
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