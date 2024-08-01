using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Dtos.User;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface CreatorInterface
    {
        string AddTicket(CreateTicketDto ticket,int id);
        public ICollection<GetTicketDto> GetMyTickets(int UserID);
        UsersDTO GetMyProfile(int userid);
        void DeleteTicket(int TicketId);
        bool Register_as_Creator(Creator creator,int id);
        bool CheckCreator(int id);
    }
}

