using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using BCrypt.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tickets_selling_App.Dtos;
using System;
using Microsoft.AspNetCore.Identity;

namespace Tickets_selling_App.Services
{
    public class UserServicre : User_Interface
    {
        private readonly Tkt_Dbcontext _context;
        private readonly IConfiguration _configuration;
        private readonly Gmail_Interface _gmail;
        public UserServicre (Tkt_Dbcontext tkt_Dbcontext, IConfiguration configuration, Gmail_Interface gmail)
        {
            _context = tkt_Dbcontext;
            _configuration = configuration;
            _gmail = gmail;
        }
        public void Registration(User customer)
        {
            if (customer != null)
            {
                var hashed = HashPassword(customer.Password);
                var Register_customer = new User
                {
                    Email = customer.Email,
                    LastName = customer.LastName,
                    Name = customer.Name,
                    Password = hashed,
                    Phone = customer.Phone,
                    Profile_Picture = customer.Profile_Picture,
                };
                _context.User.Add(Register_customer);
                _context.SaveChanges();
            } 
        }
        public string HashPassword(string password)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hashedPassword;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        public User Login(string Email, string Password)
        {
            var user = _context.User.FirstOrDefault(u => u.Email == Email);

            if (user != null)
            {
                string hashedPassword = user.Password;
                bool isPasswordCorrect = VerifyPassword(Password, hashedPassword);
                if (isPasswordCorrect)
                {
                    return user;
                }
            }
            return null;
        }
        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
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
            var user = _context.User.FirstOrDefault(x=> x.Email == mail);
            var Password_To_Reset = _context.PasswordReset.FirstOrDefault(x => x.ID == user.ID);
            if (Password_To_Reset != null && Password_To_Reset.Expiration >= DateTime.Now)
            {
                if (passcode == Password_To_Reset.Passcode)
                {
                    user.Password = password;
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
                response = "Could not find mail";
            }


            return response;
        }
    }
}
