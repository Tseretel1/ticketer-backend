using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tickets_selling_App.Dtos;
using Azure;
using Microsoft.EntityFrameworkCore;

namespace Tickets_selling_App.Services
{
    public class UserServicre : User_Interface
    {
        private readonly Tkt_Dbcontext _context;
        private readonly IConfiguration _configuration;
        private readonly Gmail_Interface _gmail;
        public UserServicre(Tkt_Dbcontext tkt_Dbcontext, IConfiguration configuration, Gmail_Interface gmail)
        {
            _context = tkt_Dbcontext;
            _configuration = configuration;
            _gmail = gmail;
        }
        public ICollection<EveryUsersDTO> AllCustomers()
        {
            var Users = _context.User.ToList();

            var UserToReturn = new List<EveryUsersDTO>();

            foreach (var x in Users)
            {
                var UserListItem = new EveryUsersDTO()
                {
                    Name = x.Name,
                    Email = x.Email,
                    LastName = x.LastName,
                    Phone = x.Phone,
                    Profile_Picture = x.Profile_Picture
                };

                UserToReturn.Add(UserListItem);
            }

            return UserToReturn;
        }

        public string Password_Restoration(string mail)
        {
            string response = "";

            try
            {
                var UserValid = _context.User.FirstOrDefault(x => x.Email == mail);

                if (UserValid != null)
                {
                    Random random = new Random();
                    int passcode = random.Next(100000, 999999);
                    if (_gmail != null)
                    {
                        _gmail.Password_Restoration(mail, passcode);
                    }
                    else
                    {
                        throw new NullReferenceException("_gmail is null. Cannot send email.");
                    }

                    var PassChange = _context.PasswordReset.FirstOrDefault(x => x.UserID == UserValid.ID);
                    if (PassChange != null)
                    {
                        PassChange.Passcode = passcode;
                        PassChange.Expiration = DateTime.Now.AddMinutes(1);
                    }
                    else
                    {
                        var PasscodeUpdate = new PasswordReset()
                        {
                            UserID = UserValid.ID,
                            Passcode = passcode,
                            Expiration = DateTime.Now.AddMinutes(1),
                        };
                        _context.PasswordReset.Add(PasscodeUpdate);
                    }

                    _context.SaveChanges();

                    response = "Passcode has been sent to your Gmail";
                }
                else
                {
                    response = "Could Not Find Mail";
                }
            }
            catch (Exception ex)
            {
                response = "An error occurred while processing your request.";
                throw;
            }

            return response;
        }

        public string Changing_Password(string mail, string password, int passcode)
        {
            string response = "";

            var user = _context.User.FirstOrDefault(x => x.Email == mail);
            if (user != null)
            {
                var PassCode_Compare = _context.PasswordReset.FirstOrDefault(x => x.ID == user.ID);
                if (PassCode_Compare != null && PassCode_Compare.Expiration >= DateTime.Now)
                {
                    if (passcode == PassCode_Compare.Passcode)
                    {
                        user.Password = password;
                        PassCode_Compare.Expiration = DateTime.Now;
                        _context.SaveChanges();
                        response = $"Your Password has changed to {password}";
                    }
                    else
                    {
                        response = "Passcode is incorrect";
                    }
                }
                else
                {
                    response = "Passcode expired try again";
                }
            }
            else
            {
                response = "Could Not find Mail";
            }
            return response;
        }
    }
}