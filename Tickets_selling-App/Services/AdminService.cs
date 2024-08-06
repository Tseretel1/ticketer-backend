using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tickets_selling_App.Dtos.TicketDTO;
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
        public string AddTicket(CreateTicketDto ticket, int id)
        {
            string response = "";
            try
            {
                if (ticket != null && ticket.Activation_Date < ticket.Expiration_Date)
                {
                    var newTicket = new Ticket
                    {
                        Title = ticket.Title,
                        Description = ticket.Description,
                        Price = ticket.Price,
                        Activation_Date = ticket.Activation_Date,
                        Expiration_Date = ticket.Expiration_Date,
                        Genre = ticket.Genre,
                        Photo = ticket.Photo,
                        PublisherID = id,
                        ViewCount = 0,
                    };

                    _context.Tickets.Add(newTicket);
                    _context.SaveChanges();

                    for (var i = 1; i <= ticket.TicketCount; i++)
                    {
                        var instances = new TicketInstance
                        {
                            Sold = false,
                            TicketID = newTicket.ID,
                            UniqueID = Guid.NewGuid().ToString(),
                        };
                        _context.TicketInstances.Add(instances);
                    }
                    _context.SaveChanges();
                    response = "Tickets have been added successfully";
                }
            }
            catch (Exception ex)
            {
                response = "Catch: " + ex.Message;
            }
            return response;
        }
        public void DeleteTicket(int TicketId)
        {
            var TicketToDelete = _context.Tickets.FirstOrDefault(x=>x.ID == TicketId);
            if (TicketToDelete != null)
            {
                var instancesToDelete = _context.TicketInstances.Where(x => x.TicketID == TicketToDelete.ID);
                _context.TicketInstances.RemoveRange(instancesToDelete);
                _context.Tickets.Remove(TicketToDelete);
                _context.SaveChanges();
            }
        }
    }
}