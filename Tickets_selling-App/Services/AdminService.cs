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
                    Response = "Tickets has been added";
                    string type = Guid.NewGuid().ToString();
                    for (int T = 1; T <= ticket.TicketCount; T++)
                    {
                        var InsertTicket = new Ticket
                        {
                            Seat = ticket.Seat,
                            Activation_Date = ticket.Activation_Date,
                            Expiration_Date = ticket.Expiration_Date,
                            Description = ticket.Description,
                            Price = ticket.Price,
                            Title = ticket.Title,
                            UniqueID = Guid.NewGuid().ToString(),
                            Type = type,
                        };
                        _context.Ticket.Add(InsertTicket);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Response = ex.Message;
            }
            return Response;
        }

        public void DeleteTicket(string Type)
        {
            if (Type != null)
            {
                var ticketsToDelete = _context.Ticket.Where(i => i.Type == Type).ToList();

                if (ticketsToDelete.Any())
                {
                    _context.Ticket.RemoveRange(ticketsToDelete);
                    _context.SaveChanges();
                }
            }
        }

        public ICollection<Ticket> GetAll_Tickets()
        {
            return _context.Ticket.ToList();
        }

        public ICollection<TicketDto> See_Tickets()
        {
            var Return_Tickets = new List<TicketDto>();

            var ASameticketsWithCount = _context.Ticket
                .GroupBy(t => t.Type)
                .Select(g => new {
                    Ticket = g.FirstOrDefault(),
                    Count = g.Count()
                })
                .ToList();

            foreach (var item in ASameticketsWithCount)
            {
                var ticket = item.Ticket;
                var count = item.Count;

                var ticketDto = new TicketDto
                {
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Seat = ticket.Seat,
                    Price = ticket.Price,
                    Activation_Date = ticket.Activation_Date,
                    Expiration_Date = ticket.Expiration_Date,
                    TicketCount = count
                };

                Return_Tickets.Add(ticketDto);
            }

            return Return_Tickets;
        }

    }
}