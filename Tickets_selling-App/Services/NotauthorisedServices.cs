using Org.BouncyCastle.Asn1.Cmp;
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
        public ICollection<GetTicketDto> AllMostPopularTickets()
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
                            TicketCount = ticket.TicketCount,
                        };
            var topTickets = query
                                .OrderByDescending(t => t.sold)
                                .Take(20)
                                .ToList();
            return topTickets;
        }


        public ICollection<GetTicketDto> MainFilter(string title)
        {
            if (title == "popular")
            {
                return AllMostPopularTickets();
            }
            else if (title == "upcoming")
            {
                return UpcomingTickets();
            }
            else if (title == "other")
            {
                return GetOtherGenreTickets();
            }
            else 
            {
                return getByGenre(title);
            }
        }


        public ICollection<GetTicketDto> MatchingTicket(int ticketId)
        {
            var targetTicket = _context.Tickets.FirstOrDefault(t => t.ID == ticketId);
            if (targetTicket == null)
            {
                return new List<GetTicketDto>();
            }
            var matchingTicketsQuery = from ticket in _context.Tickets
                                       join creator in _context.CreatorAccount on ticket.PublisherID equals creator.Id
                                       where ticket.PublisherID == targetTicket.PublisherID && ticket.Genre == targetTicket.Genre
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
                                       };

            var specifiedTicket = matchingTicketsQuery.FirstOrDefault(t => t.ID == ticketId);

            var otherTickets = matchingTicketsQuery
                               .Where(t => t.ID != ticketId)
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

        public ICollection<GetTicketDto> PopularEventsCover()
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

        public ICollection<GetTicketDto> MostPopularTickets()
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
                            TicketCount = ticket.TicketCount,
                        };
            var topTickets = query
                                .OrderByDescending(t => t.sold)
                                .Take(6)
                                .ToList();
            return topTickets;
        }

        public ICollection<GetTicketDto> GetOtherGenreTickets()
        {
            var query = from ticket in _context.Tickets
                        join creator in _context.CreatorAccount on ticket.PublisherID equals creator.Id
                        where ticket.Genre == "Other"
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
                            TicketCount = ticket.TicketCount,
                        };

            var otherGenreTickets = query
                                        .OrderByDescending(t => t.sold)
                                        .Take(20)
                                        .ToList();

            return otherGenreTickets;
        }


        public ICollection<GetTicketDto> UpcomingTickets()
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
                            TicketCount = ticket.TicketCount,
                        };
            var topTickets = query
                                .OrderByDescending(t => t.Activation_Date)
                                .Take(20)
                                .Reverse()
                                .ToList();
            return topTickets;
        }

        public ICollection<GetTicketDto> searchbyTitle(string title)
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
            var topTickets = query.Where(t=>t.Title.Contains(title))
                                .OrderByDescending(t => t.Activation_Date)
                                .Reverse()
                                .ToList();
            return topTickets;
        }

        public ICollection<GetTicketDto> getByGenre(string genre)
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
            var topTickets = query.Where(t => t.Genre == genre)
                                .OrderByDescending(t => t.Activation_Date)
                                .Reverse()
                                .ToList();
            return topTickets;
        }
    }
}
