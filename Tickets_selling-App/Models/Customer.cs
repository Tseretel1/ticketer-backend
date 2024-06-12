namespace Tickets_selling_App.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Int32 Phone { get; set; }   
        public string Profile_Picture { get; set; }
    }
}
