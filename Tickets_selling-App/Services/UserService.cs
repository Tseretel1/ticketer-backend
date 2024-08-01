using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tickets_selling_App.Dtos.User;

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
        public ICollection<UsersDTO> AllCustomers()
        {
            var Users = _context.User.ToList();

            var UserToReturn = new List<UsersDTO>();

            foreach (var x in Users)
            {
                var UserListItem = new UsersDTO()
                {
                    Name = x.Name,
                    Email = x.Email,
                    LastName = x.LastName,
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
        //Email validation----------------------------------------------
        public string Email_Validation(string Email)
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
                        NewEmailOrNot.Expiration = DateTime.Now.AddMinutes(5);
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
        //Registration----------------------------------------------
        public string Registration(RegistrationDTO user, int passcode)
        {
            try
            {
                var validEmail = _context.Emailvalidation
                    .FirstOrDefault(x => x.Email == user.Email && x.Passcode == passcode);

                if (validEmail == null)
                {
                    return "Passcode is incorrect. Please try sending it again!";
                }

                if (DateTime.Now >= validEmail.Expiration)
                {
                    return "Passcode expired. Please try again!";
                }

                var emailRegistered = _context.User.Any(x => x.Email == user.Email);
                if (emailRegistered)
                {
                    return "Email already registered.";
                }

                var hashedPassword = HashPassword(user.Password);
                var newUser = new User
                {
                    Email = user.Email,
                    LastName = user.LastName,
                    Name = user.Name,
                    Password = hashedPassword,
                    Role = "User",
                    Profile_Picture = null
                };

                _context.User.Add(newUser);
                _context.SaveChanges();

                return $"{user.Name}, you have successfully registered to our app!";
            }
            catch (Exception ex)
            {
                return $"An error occurred: {ex.Message}";
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


        //Login----------------------------------------------
        public User Login(LoginDto userDto)
        {
            var user = _context.User.FirstOrDefault(u => u.Email == userDto.Email);

            if (user != null)
            {
                string hashedPassword = user.Password;
                bool isPasswordCorrect = VerifyPassword(userDto.Password, hashedPassword);
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
                new Claim("UserID", user.ID.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
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