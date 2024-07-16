using Tickets_selling_App.Dtos;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface Admin_Interface
    {
        string AddTicket(TicketDto ticket);
        ICollection<TicketDto> GetAll_Tickets();
        void DeleteTicket (int TicketId);
    }
}
