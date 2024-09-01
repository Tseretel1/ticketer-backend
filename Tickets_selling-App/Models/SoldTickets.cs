namespace Tickets_selling_App.Models
{
    public class SoldTickets
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int TicketID { get; set; }
        public string UniqueTicketID { get; set; }
        public bool IsActive { get; set; }
    }
}
