using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Dtos.User;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface CreatorInterface
    {
        string AddTicket(CreateTicketDto ticket,int id);
        public ICollection<GetTicketDto> GetMyTickets(int UserID);
        CreatorAccount GetMyProfile(int userid);
        void DeleteTicket(int TicketId);
        public List<Ticket> MostViewed(int id);
        bool Creator_Account_Register(CreatorAccount acc,int userid);
        string Creator_Account_Login(string username, string passwrod, int userid);
    }
}

