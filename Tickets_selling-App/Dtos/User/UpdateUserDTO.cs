using Microsoft.Identity.Client;

namespace Tickets_selling_App.Dtos.User
{
    public class UpdateUserDTO
    {
        public string Name { get; set; }
        public string lastName { get;set; }
        public string photo { get; set; }
    }
}
