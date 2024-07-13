
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Services
{
    public class Login_Registration_Service :Login_Registration_Interface
    {
        private readonly Tkt_Dbcontext _context;
        private readonly Gmail_Interface _gmail;
        private readonly IConfiguration _configuration; 
        public Login_Registration_Service (Tkt_Dbcontext context, Gmail_Interface gmail, IConfiguration configuration)
        {
            _context = context;
            _gmail = gmail; 
            _configuration = configuration;
        }
        public string Registration_Validation(string Email)
        {
            string response = "";
            bool CheckEmail = _context.User.Any(x => x.Email == Email);
            if (Email != null)
            {
                if (CheckEmail)
                {
                    response = "An account has already been created using this email address. Please log in to your account or register with a different email <3 .";
                }
                else
                {
                    Random random = new Random();
                    int passcode = random.Next(100000, 999999);

                    var NewEmailOrNot = _context.Emailvalidation.FirstOrDefault(x => x.Email == Email);
                    if (NewEmailOrNot != null)
                    {
                        NewEmailOrNot.Passcode = passcode;
                        NewEmailOrNot.Expiration = DateTime.Now.AddMinutes(2);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var ValidateEmail = new Email_Validation
                        {
                            Email = Email,
                            Passcode = passcode,
                            Expiration = DateTime.Now.AddMinutes(2),
                        };
                        _context.Emailvalidation.Add(ValidateEmail);
                        _context.SaveChanges();
                    }
                    _gmail.Email_Validation(Email, passcode);
                    response = "We have sent you 6 digits passcode to your email address, please enter below to finish registration!";
                }
            }
            return response;
        }

        public string Registration(User user, int passcode)
        {
            string response = "";
            try
            {
                var CheckEmail = _context.Emailvalidation.FirstOrDefault(x => x.Email == user.Email && x.Passcode == passcode);
                if (CheckEmail != null)
                {
                    if (CheckEmail.Expiration > DateTime.Now)
                    {
                        if (user != null)
                        {
                            response = $"{user.Name} You successfully registered to our app!";
                            var hashed = HashPassword(user.Password);
                            var Register_customer = new User
                            {
                                Email = user.Email,
                                LastName = user.LastName,
                                Name = user.Name,
                                Password = hashed,
                                Phone = user.Phone,
                                Profile_Picture = null,
                            };
                            _context.User.Add(Register_customer);
                            _context.Emailvalidation.Remove(CheckEmail);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        response = "Passcode expired please try again";
                    }
                }
                else
                {
                    response = "Passcode is incorrect please try sending it again!";
                }
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }
            return response;
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
    }
}
