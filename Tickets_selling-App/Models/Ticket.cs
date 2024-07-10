﻿
namespace Tickets_selling_App.Models
{
    public class Ticket
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Seat { get; set; }
        public int Price { get; set; }
        public DateTime Activation_Date { get; set; }
        public DateTime Expiration_Date { get; set; }
        public string UniqueID { get; set; }
        public string Type { get; set; }
    }
}
