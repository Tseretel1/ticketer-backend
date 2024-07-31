namespace Tickets_selling_App.Models
{
    public class Creator
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public long PersonalID { get; set; }
        public long PhoneNumber { get; set; }
        public string IdCardPhoto { get; set; }
        public bool Verified { get; set; }
    }
}
