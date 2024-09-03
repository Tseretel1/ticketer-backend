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
                        join soldTickets in _context.SoldTickets
                            on ticket.ID equals soldTickets.TicketID into soldTicketGroup
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
                            TicketCount = ticket.TicketCount,
                            sold = soldTicketGroup.Count(),
                            Publisher = new CreatorAccountDTO
                            {
                                UserName = creator.UserName,
                                Logo = creator.Logo,
                                id = ticket.PublisherID,
                            },
                            ViewCount = ticket.ViewCount,
                        };

            var sortedQuery = query.OrderByDescending(t => t.Activation_Date);

            return sortedQuery.ToList();
        }



        public ICollection<GetTicketDto> MatchingTicket(int ticketid)
        {
            var targetTicket = _context.Tickets.FirstOrDefault(t => t.ID == ticketid);
            if (targetTicket == null)
            {
                return new List<GetTicketDto>();
            }
            var matchingTicketsQuery = (from ticket in _context.Tickets
                                        join creator in _context.CreatorAccount on ticket.PublisherID equals creator.Id
                                        where ticket.PublisherID == targetTicket.PublisherID
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
                                            TicketCount = ticket.TicketCount,
                                            Publisher = new CreatorAccountDTO
                                            {
                                                UserName = creator.UserName,
                                                Logo = creator.Logo,
                                                id = ticket.PublisherID,
                                            },
                                            ViewCount = ticket.ViewCount,
                                        });
            var specifiedTicket = matchingTicketsQuery.FirstOrDefault(t => t.ID == ticketid);
            var otherTickets = matchingTicketsQuery
                               .Where(t => t.ID != ticketid)
                               .Take(4) 
                               .ToList();

            if (specifiedTicket != null)
            {
                otherTickets.Insert(0, specifiedTicket);
            }

            return otherTickets;
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
                            sold = _context.SoldTickets.Where(x => x.TicketID == ticket.ID).Count(),
                            Publisher = new CreatorAccountDTO
                            {
                                UserName = creator.UserName,
                                Logo = creator.Logo,
                            },
                            ViewCount = ticket.ViewCount,
                        };
            var topTickets = query
                                .OrderByDescending(t => t.sold)
                                .Take(5)
                                .ToList();
            return topTickets;
        }
    }
}
