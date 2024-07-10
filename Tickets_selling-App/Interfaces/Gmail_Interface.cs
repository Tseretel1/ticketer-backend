

namespace Tickets_selling_App.Interfaces
{
    public interface Gmail_Interface
    {
        public Task SendEmailAsync(string email,string qrCodeData);
        public Task Password_Restoration (string mail, int Passcode);
        public Task Email_Validation(string email,int passcode);
        public byte[] QrGenerator(string data);
    }
}
