using Microsoft.AspNetCore.Mvc;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Services
{
    public class AdminService : Admin_Interface
    {
        private readonly Tkt_Dbcontext _context;
        
        public AdminService(Tkt_Dbcontext context)
        {
            _context = context;
        }
        public void AddTicket(Ticket ticket)
        {
            if(ticket != null)
            {
                if(ticket.Activation_Date < ticket.Expiration_Date)
                {
                   _context.Ticket.Add(ticket);
                   _context.SaveChanges();
                }
            }
        }

        public void DeleteTicket(int id)
        {
            if (id != null)
            {
                var Ticket_To_delete = _context.Ticket.FirstOrDefault(i => i.ID == id);
                if(Ticket_To_delete != null)
                {
                    _context.Ticket.Remove(Ticket_To_delete);
                    _context.SaveChanges();
                }
            }
        }

        public ICollection<Ticket> GetAll_Tickets()
        {
            return _context.Ticket.ToList();
        }
    }
}
