using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using QRCoder;
using Tickets_selling_App.Interfaces;

namespace Tickets_selling_App.Services
{
    public class GmailService : Gmail_Interface
    {
        public async Task SendEmailAsync(string email, string subject, string message, string qrCodeData)
        {
            try
            {
                var client = new SmtpClient("live.smtp.mailtrap.io", 587)
                {
                    Credentials = new NetworkCredential("api", "c0a4651a8cd800b259893fd94e8d1856"),
                    EnableSsl = true
                };

                var fromAddress = new MailAddress("mailtrap@demomailtrap.com", "Ticket");
                var toAddress = new MailAddress(email);

                var mailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = false,
                };

                // Generate QR code image bytes
                byte[] qrCodeBytes = QrGenerator(qrCodeData);

                // Attach QR code image to the email
                using (var qrStream = new MemoryStream(qrCodeBytes))
                {
                    mailMessage.Attachments.Add(new Attachment(qrStream, "qrcode.png", "image/png"));
                    await client.SendMailAsync(mailMessage);
                }

                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in SendEmailAsync: {ex.Message}");
                throw;
            }
        }

        public byte[] QrGenerator(string data)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q))
            using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
            {
                return qrCode.GetGraphic(20); // 20 is pixel size
            }
        }
    }
}
