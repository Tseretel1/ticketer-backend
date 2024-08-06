using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Services
{
    public class NotauthorisedServices : NotauthorisedInterface
    {
        private readonly Tkt_Dbcontext _context;
        public NotauthorisedServices(Tkt_Dbcontext context)
        {
            _context = context;
        }
        public ICollection<GetTicketDto> GetAll_Tickets()
        {
            var query = from ticket in _context.Tickets
                        join creator in _context.CreatorAccount on ticket.PublisherID equals creator.Id
                        let ticketInstancesCount = _context.TicketInstances.Count(ti => ti.Sold == false && ti.TicketID == ticket.ID)
                        select new GetTicketDto
                        {
                            ID = ticket.ID,
                            Activation_Date = ticket.Activation_Date,
                            Description = ticket.Description,
                            Expiration_Date = ticket.Expiration_Date,
                            Genre = ticket.Genre,
                            Photo = ticket.Photo,
                            Price = ticket.Price,
                            Title = ticket.Title,
                            TicketCount = ticketInstancesCount,
                            Publisher = new CreatorAccountDTO
                            {
                                UserName = creator.UserName,
                                Logo = creator.Logo,
                            },
                            ViewCount = ticket.ViewCount,
                        };

            return query.ToList();
        }



        public bool PlusViewCount(int id)
        {
            var Ticket = _context.Tickets.FirstOrDefault(x=> x.ID == id);
            if(Ticket!=null)
            {
                Ticket.ViewCount += 1;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public ICollection<GetTicketDto> PopularEvents()
        {
            var query = from ticket in _context.Tickets
                        join creator in _context.CreatorAccount on ticket.PublisherID equals creator.Id
                        let ticketInstancesCount = _context.TicketInstances.Count(ti => ti.Sold == false && ti.TicketID == ticket.ID)
                        select new GetTicketDto
                        {
                            ID = ticket.ID,
                            Activation_Date = ticket.Activation_Date,
                            Description = ticket.Description,
                            Expiration_Date = ticket.Expiration_Date,
                            Genre = ticket.Genre,
                            Photo = ticket.Photo,
                            Price = ticket.Price,
                            Title = ticket.Title,
                            TicketCount = ticketInstancesCount,
                            Publisher = new CreatorAccountDTO
                            {
                                UserName = creator.UserName,
                                Logo = creator.Logo,
                            },
                            ViewCount = ticket.ViewCount,
                        };
            var topTickets = query
                                .OrderByDescending(t => t.ViewCount)
                                .Take(5)
                                .ToList();
            return topTickets;
        }
    }
}
