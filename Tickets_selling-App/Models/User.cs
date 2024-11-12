using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Tickets_selling_App.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? Password { get; set; }
        public string? Photo { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? PersonalID { get; set; }
        public string? PhoneNumber { get; set; }
        public string? IdCardPhoto { get; set; }
    }
}
