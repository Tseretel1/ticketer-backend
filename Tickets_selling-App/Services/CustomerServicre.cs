using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using BCrypt.Net;
using System.Threading.Tasks;

namespace Tickets_selling_App.Services
{
    public class CustomerServicre : Customer_Interface
    {
        private readonly Tkt_Dbcontext _context;
        public CustomerServicre (Tkt_Dbcontext tkt_Dbcontext)
        {
             _context = tkt_Dbcontext;
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
                _context.Customer.Add(Register_customer);
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

        public ICollection<User> AllCustomers()
        {
            return _context.Customer.ToList();
        }
    }
}
