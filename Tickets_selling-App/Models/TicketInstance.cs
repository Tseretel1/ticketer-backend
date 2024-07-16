namespace Tickets_selling_App.Models
{
    public class TicketInstance
    {
        public int ID { get; set; }
        public int TicketID { get; set; }
        public string UniqueID { get; set; }
        public bool Sold { get; set; }
    }
}
