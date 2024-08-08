namespace Tickets_selling_App.Models
{
    public class EmailValidation
    {
        public int id { get; set; }
        public string Email { get; set; }
        public int Passcode { get; set; }
        public DateTime Expiration { get; set; }
    }
}
