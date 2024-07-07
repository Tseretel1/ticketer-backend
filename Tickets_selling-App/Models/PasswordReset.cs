using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tickets_selling_App.Models
{
    public class PasswordReset
    { 
        public int ID { get; set; }
        public int UserID { get; set; }
        public int Passcode { get; set; }
        public DateTime Expiration { get; set; }
    }

}
