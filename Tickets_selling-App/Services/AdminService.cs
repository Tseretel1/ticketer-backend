using Microsoft.AspNetCore.Mvc;
using Tickets_selling_App.Dtos;
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
        public string AddTicket(TicketDto ticket)
        {
            string Response = "";
            try
            {
                if (ticket != null && ticket.Activation_Date < ticket.Expiration_Date)
                {
                    var AddTicket = new Ticket
                    {
                        Activation_Date = ticket.Activation_Date,
                        Expiration_Date = ticket.Expiration_Date,
                        Description = ticket.Description,
                        Price = ticket.Price,
                        Title = ticket.Title,
                        Photo = ticket.Photo,
                        Genre = ticket.Genre,
                    };

                    _context.Tickets.Add(AddTicket);
                    _context.SaveChanges();
                    for (var i = 1; i <= ticket.TicketCount; i++)
                    {
                        var TicketInstance = new TicketInstance
                        {
                            TicketID = AddTicket.ID,
                            UniqueID = Guid.NewGuid().ToString(),
                            Sold = false,
                        };
                        _context.TicketInstances.Add(TicketInstance);
                    }

                    _context.SaveChanges();

                    Response = "Tickets have been added";
                }
            }
            catch (Exception ex)
            {
                Response = ex.Message;
            }
            return Response;
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

        public ICollection<TicketDto> GetAll_Tickets()
        {
            var Ticket = _context.Tickets.ToList();
            var TicketDTo = new List<TicketDto>();
            foreach (var x in Ticket)
            {
                var TicketInstances = _context.TicketInstances.Where(x => x.Sold == false).Count();
                TicketDto TicketD = new TicketDto()
                {
                    ID = x.ID,
                    Activation_Date = x.Activation_Date,
                    Description = x.Description,
                    Expiration_Date = x.Expiration_Date,
                    Genre = x.Genre,
                    Photo = x.Photo,
                    Price = x.Price,
                    Title = x.Title,
                    TicketCount = TicketInstances,
                };
                TicketDTo.Add(TicketD);
            }
            return TicketDTo;
        }
    }
}