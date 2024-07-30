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
            var Ticket = _context.Tickets.ToList();
            var TicketDTo = new List<GetTicketDto>();
            foreach (var x in Ticket)
            {
                var user = _context.User.FirstOrDefault(u => u.ID == x.PublisherID);
                var Publisher = new CreatorDTO
                {
                    Email = user.Email,
                    LastName = user.LastName,
                    Name = user.Name,
                    Phone = user.Phone,
                    Profile = user.Profile_Picture,
                };
                var TicketInstances = _context.TicketInstances.Where(t => t.Sold == false && t.TicketID == x.ID).Count();
                GetTicketDto TicketD = new GetTicketDto()
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
                    Publisher = Publisher,
                    ViewCount = x.ViewCount,
                };
                TicketDTo.Add(TicketD);
            }
            return TicketDTo;
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
            var tickets = _context.Tickets.ToList();
            var ticketDtos = new List<GetTicketDto>();

            foreach (var ticket in tickets)
            {
                var unsoldTicketCount = _context.TicketInstances
                                               .Where(t => ticket.ID == t.TicketID && t.Sold == false)
                                               .Count();
                var user = _context.User.FirstOrDefault(u => u.ID == ticket.PublisherID);
                var Publisher = new CreatorDTO
                {
                    Email = user.Email,
                    LastName = user.LastName,
                    Name = user.Name,
                    Phone = user.Phone,
                    Profile = user.Profile_Picture,
                };
                var ticketDto = new GetTicketDto
                {
                    ID = ticket.ID,
                    Activation_Date = ticket.Activation_Date,
                    Description = ticket.Description,
                    Expiration_Date = ticket.Expiration_Date,
                    Genre = ticket.Genre,
                    Photo = ticket.Photo,
                    Price = ticket.Price,
                    Title = ticket.Title,
                    TicketCount = unsoldTicketCount,
                    Publisher = Publisher,
                };
                ticketDtos.Add(ticketDto);
            }
            var topTickets = ticketDtos
                                .OrderByDescending(t => t.TicketCount)
                                .Take(5)
                                .ToList();
            return topTickets;
        }
    }
}
