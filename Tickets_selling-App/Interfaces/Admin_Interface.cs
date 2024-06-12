using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface Admin_Interface
    {
        void AddTicket(Ticket ticket);
        ICollection<Ticket> GetAll_Tickets();
        void DeleteTicket (int ID);  
    }
}
