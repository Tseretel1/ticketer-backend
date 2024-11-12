namespace Tickets_selling_App.Dtos.Creator
{
    public class AccountManagment
    {
        public int UserID { get; set; } 
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Photo { get; set; }
        public string Email { get; set; }   
        public string AccountRole { get; set; }
        public string? phoneNumber { get; set; }
        public string? PersonalID { get; set; }
    }
}
