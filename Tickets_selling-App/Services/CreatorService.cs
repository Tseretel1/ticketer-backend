﻿using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tickets_selling_App.Dtos.Creator;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Services
{
    public class CreatorService : CreatorInterface
    {
        private readonly Tkt_Dbcontext _context;
        private readonly IConfiguration _configuration;
        public CreatorService(Tkt_Dbcontext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        //Registration____login   
        public bool register_as_creator(int userid, RegisterAsCreatorDTO cred)
        {
            bool happens = false;
            var user = _context.User.FirstOrDefault(x => x.ID == userid);

            if (user != null)
            {
                if (cred.IdCardPhoto != null && cred.PhoneNumber != null && cred.PersonalID != null)
                {
                    user.PersonalID = cred.PersonalID;
                    user.PhoneNumber = cred.PhoneNumber;
                    user.IdCardPhoto = cred.IdCardPhoto;
                    _context.SaveChanges();
                    happens = true;
                }
                if (happens)
                {
                    var alreadyRegistered = _context.CreatorValidation.FirstOrDefault(x=>x.ID == userid);
                    if (alreadyRegistered != null)
                    {
                        var CreatorValidation = new CreatorValidation()
                        {
                            Userid = userid,
                            Verified = false,
                        };

                        _context.CreatorValidation.Add(CreatorValidation);
                        _context.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }
        public bool Creator_Account_Register(string accName, int userid)
        {
            try
            {
                var creator = _context.User.FirstOrDefault(x => x.ID == userid);
                if (creator != null)
                {
                    var accountExists = _context.CreatorAccount
                        .FirstOrDefault(x => x.UserName == accName && x.CreatorID == userid);

                    if (accountExists == null)
                    {
                        var newAccount = new CreatorAccount
                        {
                            CreatorID = userid,
                            Logo = "",
                            UserName = accName,
                        };

                        _context.CreatorAccount.Add(newAccount);
                        _context.SaveChanges();
                        var createdAccount = _context.CreatorAccount
                            .FirstOrDefault(x => x.UserName == accName && x.CreatorID == userid);

                        if (createdAccount != null)
                        {
                            var role = new CreatorAccountRoles()
                            {
                                AccountID = createdAccount.Id,
                                Role = "Creator",
                                UserID = userid,
                            };
                            _context.AccountRoles.Add(role);
                            _context.SaveChanges();
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public CreatorAccount createdAccountCredentials(string accName, int creatorid )
        {
            try
            {
                var creator = _context.CreatorAccount.FirstOrDefault(x => x.UserName == accName && x.CreatorID == creatorid);
                if (creator == null)
                {
                    return null;
                }
                return creator;
            }
            catch
            {
                return null;
            }
        }

        public string Creator_Account_Login(int accountid, int userid)
        {
            var UserPermission = _context.AccountRoles.FirstOrDefault(x => x.UserID == userid && x.AccountID == accountid);
            if (UserPermission == null)
            {
                return null;
            }
            var Token = AccountRoleToken(UserPermission);
            return Token;
        }
        public ICollection<CreatorAccount> myCreatorAccounts(int userId)
        {
            var accountIds = _context.AccountRoles
                .Where(ar => ar.UserID == userId)
                .Select(ar => ar.AccountID)
                .Distinct()
                .ToList();

            if (!accountIds.Any())
            {
                return new List<CreatorAccount>();
            }

            var creatorAccounts = _context.CreatorAccount
                .Where(ca => accountIds.Contains(ca.Id))
                .ToList();

            return creatorAccounts;
        }



        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public string AccountRoleToken(CreatorAccountRoles acc)
        {
            var claims = new List<Claim>
            {
                new Claim("AccountID" , acc.AccountID.ToString()),
                new Claim("UserID" , acc.UserID.ToString()),
                new Claim(ClaimTypes.Role, acc.Role),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        //  ---------------------Managment services  --------------------------
        public ICollection<GetTicketDto> GetMyActiveTickets(int AccountID, int pageindex)
        {
            int GetticketCount = 10;
            int skipcount = pageindex * GetticketCount;
            var query = from ticket in _context.Tickets.OrderByDescending(x => x.Activation_Date)
                        join creator in _context.CreatorAccount
                        on ticket.PublisherID equals creator.Id
                        where ticket.PublisherID == AccountID
                        let SoldTicketCount = _context.SoldTickets.Count(ti => ti.TicketID == ticket.ID)
                        where ticket.PublisherID == AccountID
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
                            sold = SoldTicketCount,
                            Publisher = new CreatorAccountDTO
                            {
                                UserName = creator.UserName,
                                Logo = creator.Logo,
                            },
                            ViewCount = ticket.ViewCount,
                            TicketCount = ticket.TicketCount,
                        };

            return query.Where(x => x.Activation_Date > DateTime.Now).Skip(skipcount).Take(GetticketCount).ToList();
        }
        public ICollection<GetTicketDto> GetMyExpiredTickets(int AccountID, int pageindex)
        {
            int GetticketCount = 10;
            int skipcount = pageindex * GetticketCount;
            var query = from ticket in _context.Tickets
                        join creator in _context.CreatorAccount
                        on ticket.PublisherID equals creator.Id
                        where ticket.PublisherID == AccountID && ticket.Expiration_Date < DateTime.Now
                        let SoldTicketCount = _context.SoldTickets.Count(ti => ti.TicketID == ticket.ID)
                        orderby ticket.Activation_Date descending 
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
                            sold = SoldTicketCount,
                            Publisher = new CreatorAccountDTO
                            {
                                UserName = creator.UserName,
                                Logo = creator.Logo,
                            },
                            ViewCount = ticket.ViewCount,
                            TicketCount = ticket.TicketCount,
                        };
            return query.Where(t=>t.Expiration_Date < DateTime.Now).Skip(skipcount).Take(GetticketCount).ToList();
        }


        public object GetMyProfile(int AccountID, int userid)
        {
            var acc = _context.CreatorAccount.FirstOrDefault(x => x.Id == AccountID);
            var user = _context.User.FirstOrDefault(x => x.ID == userid);
            var role = _context.AccountRoles.FirstOrDefault(x => x.AccountID == AccountID && x.UserID == userid);
            if (acc != null && user != null && role !=null)
            {
                var combinedProfile = new
                {
                    Logo = acc.Logo,
                    UserName = acc.UserName,
                    Name = user.Name,
                    LastName = user.LastName,
                    UserRole = role.Role,
                };

                return combinedProfile;
            }
            return null;
        }


        public ICollection<AccountManagment> GetManagement(int accountId)
        {
            var account = _context.CreatorAccount.FirstOrDefault(x => x.Id == accountId);
            if (account == null)
            {
                return new List<AccountManagment>();
            }

            var accountRoles = _context.AccountRoles.Where(x => x.AccountID == accountId).ToList();
            if (accountRoles.Count == 0)
            {
                return new List<AccountManagment>();
            }
            var managementList = new List<AccountManagment>();

            foreach (var role in accountRoles)
            {
                var user = _context.User.FirstOrDefault(x => x.ID == role.UserID);
                if (user != null)
                {
                    var accountManagement = new AccountManagment
                    {
                        AccountRole = role.Role,
                        Email = user.Email,
                        LastName = user.LastName,
                        Name = user.Name,
                        UserID = user.ID,
                        phoneNumber = user.PhoneNumber,
                        PersonalID = user.PersonalID,
                    };
                    managementList.Add(accountManagement);
                }
            }

            return managementList;
        }

        public bool RemoveUser(int userid)
        {
            var userToRemove = _context.AccountRoles.FirstOrDefault(x=> x.UserID == userid);
            if(userToRemove != null)
            {
                _context.AccountRoles.Remove(userToRemove);
                _context.SaveChanges();
                return true;
            }
            return false;
        }







        //Ticket Services
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
                        TicketCount = ticket.TicketCount,
                    };

                    _context.Tickets.Add(newTicket);
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
        public string UpdateTicket(CreateTicketDto ticket)
        {
            string response;

            try
            {
                if (ticket == null)
                {
                    return "Error: Ticket data is null";
                }

                var TicketToUpdate = _context.Tickets.SingleOrDefault(x => x.ID == ticket.ID);

                if (TicketToUpdate == null)
                {
                    return "Error: Ticket not found";
                }
                    TicketToUpdate.Title = ticket.Title;
                    TicketToUpdate.Price = ticket.Price;
                    TicketToUpdate.Activation_Date = ticket.Activation_Date;
                    TicketToUpdate.Expiration_Date = ticket.Expiration_Date;
                    TicketToUpdate.Description = ticket.Description;
                    TicketToUpdate.Genre = ticket.Genre;
                    TicketToUpdate.Photo = ticket.Photo;
                    TicketToUpdate.TicketCount = ticket.TicketCount;
                    _context.SaveChanges();

                response = "Ticket has been modified successfully";
            }
            catch (Exception ex)
            {
                response = $"Error: {ex.Message}";
            }

            return response;
        }

        public bool DeleteTicket(int TicketId)
        {
            var TicketToDelete = _context.Tickets.FirstOrDefault(x => x.ID == TicketId);
            if (TicketToDelete != null)
            {
                _context.Tickets.Remove(TicketToDelete);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public GetTicketDto MatchingTicket(int ticketid)
        {
            var targetTicket = _context.Tickets
                .Join(_context.CreatorAccount,
                      ticket => ticket.PublisherID,
                      creator => creator.Id,
                      (ticket, creator) => new { ticket, creator })
                .Where(tc => tc.ticket.ID == ticketid)
                .Select(tc => new GetTicketDto
                {
                    ID = tc.ticket.ID,
                    Activation_Date = tc.ticket.Activation_Date,
                    Description = tc.ticket.Description,
                    Expiration_Date = tc.ticket.Expiration_Date,
                    Genre = tc.ticket.Genre,
                    Photo = tc.ticket.Photo,
                    Price = tc.ticket.Price,
                    Title = tc.ticket.Title,
                    Publisher = new CreatorAccountDTO
                    {
                        UserName = tc.creator.UserName,
                        Logo = tc.creator.Logo,
                        id = tc.ticket.PublisherID,
                    },
                    ViewCount = tc.ticket.ViewCount,
                })
                .FirstOrDefault();

            return targetTicket;
        }






        //Managment services
        public List<Ticket> MostViewed(int id)
        {
            List<Ticket> MostViewedTickets = new List<Ticket>();
            var tickets = _context.Tickets.Where(x => x.PublisherID == id).ToList();

            if (tickets != null && tickets.Any())
            {
                MostViewedTickets = tickets
                    .OrderByDescending(t => t.ViewCount)
                    .Take(5)
                    .ToList();
                return MostViewedTickets;
            }
            return null;
        }




        //Qr Scanner Services
        public ScanedTicketDTO ScanTicket(string ticketid)
        {
            var FoundTicket = _context.SoldTickets.FirstOrDefault(x => x.UniqueTicketID == ticketid);
            if (FoundTicket == null)
            {
                var news = new ScanedTicketDTO
                {
                    isExpired = false,
                    TicketTitle = "ticket Could not be found",
                };
                return news;
            }
            var FoundUser = _context.User.FirstOrDefault(x => x.ID == FoundTicket.UserID);
            if (FoundUser != null)
            {
                var Ticket = _context.Tickets.FirstOrDefault(x => x.ID == FoundTicket.TicketID);
                bool Expired = false;   
                if(Ticket.Expiration_Date < DateTime.Now)
                {
                    Expired = true;
                };
                var Ticketqrd = new ScanedTicketDTO
                {
                    UserName = FoundUser.Name + " " + FoundUser.LastName,
                    isExpired = Expired,
                    IsActive = FoundTicket.IsActive,
                    TicketTitle = Ticket.Title,
                    ActivationDate = Ticket.Activation_Date,
                    ExpirationDate = Ticket.Expiration_Date,
                    TicketPhoto = Ticket.Photo,
                };
      
                return Ticketqrd;
            }      
            return null;
        }
    }
}
