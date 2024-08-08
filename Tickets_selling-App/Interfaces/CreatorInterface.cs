using Tickets_selling_App.Dtos.Creator;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface CreatorInterface
    {
        string AddTicket(CreateTicketDto ticket,int id);
        public ICollection<GetTicketDto> GetMyTickets(int UserID);
        object GetMyProfile(int userid,int Userid);
        void DeleteTicket(int TicketId);
        public List<Ticket> MostViewed(int id);
        bool register_as_creator(int userid,RegisterAsCreatorDTO cred);
        bool Creator_Account_Register(CreatorAccount acc,int userid);
        string Creator_Account_Login(string username, string passwrod, int userid);
    }
}

