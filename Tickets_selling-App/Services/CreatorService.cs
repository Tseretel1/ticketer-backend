using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Dtos.User;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Services
{
    public class CreatorService : CreatorInterface
    {
        private readonly Tkt_Dbcontext _context;

        public CreatorService(Tkt_Dbcontext context)
        {
            _context = context;
        }

        public ICollection<GetTicketDto> GetMyTickets(int UserID)
        {
            var Ticket = _context.Tickets.Where(x => x.PublisherID == UserID).ToList();
            var TicketDTo = new List<GetTicketDto>();
            foreach (var x in Ticket)
            {
                var user = _context.User.FirstOrDefault(u => u.ID == UserID);
                var Publisher = new CreatorDTO
                {
                    Email = user.Email,
                    LastName = user.LastName,
                    Name = user.Name,
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
        public UsersDTO GetMyProfile(int userid) 
        {
            var user = _context.User.FirstOrDefault(x => x.ID == userid);
            if (user != null)
            {
                var FoundUser = new UsersDTO
                {
                    Name = user.Name,
                    Email = user.Email,
                    LastName = user.LastName,
                    UserRole = user.Role,
                    Profile_Picture = user.Profile_Picture,
                };
                return FoundUser;
            }       
            return null;
        }
        public string AddTicket(CreateTicketDto ticket, int id)
        {
            string response = "";
            try
            {
                if (ticket != null)
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
            var TicketToDelete = _context.Tickets.FirstOrDefault(x => x.ID == TicketId);
            if (TicketToDelete != null)
            {
                var instancesToDelete = _context.TicketInstances.Where(x => x.TicketID == TicketToDelete.ID);
                _context.TicketInstances.RemoveRange(instancesToDelete);
                _context.Tickets.Remove(TicketToDelete);
                _context.SaveChanges();
            }
        }

        public bool Register_as_Creator(Creator Creator,int id)
        {
            var Registered = _context.Creator.FirstOrDefault(x=>x.UserID == id ||  x.PersonalID == Creator.PersonalID);
            if(Registered == null)
            {
                var Newcreator = new Creator
                {
                    IdCardPhoto = Creator.IdCardPhoto,
                    PersonalID = Creator.PersonalID,
                    PhoneNumber = Creator.PhoneNumber,
                    UserID = id,
                    Verified = false,
                };
                _context.Creator.Add(Newcreator);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
