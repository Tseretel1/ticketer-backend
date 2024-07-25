using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface Admin_Interface
    {
        string AddTicket(CreateTicketDto ticket);
        void DeleteTicket (int TicketId);
    }
}
