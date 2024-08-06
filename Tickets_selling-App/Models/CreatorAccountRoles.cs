using Org.BouncyCastle.Tls.Crypto;
using System.Runtime.CompilerServices;

namespace Tickets_selling_App.Models
{
    public class CreatorAccountRoles
    {
        public int id { get; set; }
        public int UserID { get; set; }
        public int AccountID { get; set; }
        public string Role { get; set; }
    }
}
