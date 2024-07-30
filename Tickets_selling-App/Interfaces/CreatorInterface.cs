using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Dtos.User;

namespace Tickets_selling_App.Interfaces
{
    public interface CreatorInterface
    {
        string AddTicket(CreateTicketDto ticket,int id);
        public ICollection<GetTicketDto> GetMyTickets(int UserID);
        UsersDTO GetMyProfile(int userid);
        void DeleteTicket(int TicketId);
    }
}

