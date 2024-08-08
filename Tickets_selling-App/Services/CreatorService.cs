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

        public bool Creator_Account_Register(CreatorAccount acc,int userid)
        {
            try
            {
                var creator = _context.User.FirstOrDefault(x => x.ID == userid);
                if (creator != null)
                {
                    var AccountExists = _context.CreatorAccount.FirstOrDefault(x => x.UserName == acc.UserName);
                    if (AccountExists == null) {
                        var HashedPassword = HashPassword(acc.Password);
                        var newAccount = new CreatorAccount
                        {
                            CreatorID = userid,
                            Logo = acc.Logo,
                            Password = HashedPassword,
                            UserName = acc.UserName,
                        };
                        _context.CreatorAccount.Add(newAccount);
                        _context.SaveChanges(); 
                    }
                    else
                    {
                        return false;
                    }
                }
                var account = _context.CreatorAccount.FirstOrDefault(x=>x.CreatorID == acc.CreatorID);
                var Role = new CreatorAccountRoles()
                {
                    AccountID = account.Id,
                    Role = "CreatorAdmin",
                    UserID = userid,
                };
                _context.AccountRoles.Add(Role);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hashedPassword;
        }



        public string Creator_Account_Login(string username, string password, int userid)
        {
            var Account = _context.CreatorAccount.FirstOrDefault(u => u.UserName == username);
            if (Account == null)
            {
                return null;
            }

            bool isPasswordCorrect = VerifyPassword(password, Account.Password);
            if (!isPasswordCorrect)
            {
                return null;
            }

            var UserPermission = _context.AccountRoles.FirstOrDefault(x => x.UserID == userid && x.AccountID == Account.Id);
            if (UserPermission == null)
            {
                return null;
            }

            var Token = AccountRoleToken(UserPermission);
            return Token;
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

        //  ---------------------Services --------------------------
        public ICollection<GetTicketDto> GetMyTickets(int AccountID)
        {
            var query = from ticket in _context.Tickets
                        join creator in _context.CreatorAccount
                        on ticket.PublisherID equals creator.Id
                        let ticketInstancesCount = _context.TicketInstances.Count(ti => ti.Sold == false && ti.TicketID == ticket.ID)
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
                    Profile = user.Profile_Picture,
                    UserRole = role.Role,
                };

                return combinedProfile;
            }

            return null;
        }



        //ticket crud
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
    }
}
