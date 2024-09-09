namespace Tickets_selling_App.Dtos.Creator
{
    public class AccountManagment
    {
        public int UserID { get; set; } 
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Profile { get; set; }
        public string Email { get; set; }   
        public string AccountRole { get; set; }
        public long? phoneNumber { get; set; }
        public long? PersonalID { get; set; }
    }
}
