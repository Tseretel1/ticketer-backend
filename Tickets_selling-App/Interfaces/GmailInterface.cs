

namespace Tickets_selling_App.Interfaces
{
    public interface GmailInterface
    {
        public Task TicketBought(string email);
        public Task Email_Validation(string email,int passcode);
    }
}
