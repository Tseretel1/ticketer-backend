using System;
using System.Drawing;

namespace Tickets_selling_App.Interfaces
{
    public interface Gmail_Interface
    {
        public Task SendEmailAsync(string email, string subject, string message, string qrCodeData);
        byte[] QrGenerator(string data);
    }
}
