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

namespace Tickets_selling_App.Services
{
    public class UserServicre : User_Interface
    {
        private readonly Tkt_Dbcontext _context;
        private readonly IConfiguration _configuration;
        public UserServicre (Tkt_Dbcontext tkt_Dbcontext, IConfiguration configuration)
        {
             _context = tkt_Dbcontext;
            _configuration = configuration;
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

    }
}
