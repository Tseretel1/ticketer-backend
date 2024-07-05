using Tickets_selling_App.Dtos;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface Admin_Interface
    {
        string AddTicket(TicketDto ticket);
        ICollection<Ticket> GetAll_Tickets();
        void DeleteTicket (string Type);
        ICollection <TicketDto> See_Tickets();
    }
}
