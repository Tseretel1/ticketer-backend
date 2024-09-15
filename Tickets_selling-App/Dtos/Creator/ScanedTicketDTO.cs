using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Dtos.Creator
{
    public class ScanedTicketDTO
    {
        public string UserName { get; set; }
        public string TicketTitle { get; set; }
        public bool isExpired { get; set; }
        public bool IsActive { get; set; }
        public DateTime ActivationDate{ get; set; }
        public DateTime ExpirationDate { get; set; }
        public string TicketPhoto { get; set; }
    }
}
