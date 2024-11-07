

using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface GmailInterface
    {
        Task TicketBought(string email, int boughtCount, Ticket ticket);
        public Task Email_Validation(string email,int passcode);
    }
}
