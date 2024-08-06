using System.Runtime.CompilerServices;

namespace Tickets_selling_App.Models
{
    public class CreatorAccount
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Logo { get; set; }
        public string Password { get; set; }
        public int CreatorID { get; set;}
    }
}
