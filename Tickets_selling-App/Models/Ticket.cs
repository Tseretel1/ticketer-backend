using System.ComponentModel.DataAnnotations.Schema;

namespace Tickets_selling_App.Models
{
    public class Ticket
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Price { get; set; }
        public DateTime Activation_Date { get; set; }
        public DateTime Expiration_Date { get; set; }
        public string Genre { get; set; }
        public string Photo { get; set; }
        public int ViewCount { get; set; }
        public int PublisherID { get; set; }
    }
}
