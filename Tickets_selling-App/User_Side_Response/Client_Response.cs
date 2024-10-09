using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tickets_selling_App.User_Side_Response
{
    [NotMapped]
    public class Client_Response
    {
        public string Message { get; set; }
        public bool Success { get; set; }
        public int? accountID { get;set;}
    }
}
