using Tickets_selling_App.Dtos.Ticket;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Dtos.User;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface UserInterface
    {
        ICollection<UsersDTO> AllCustomers();
        bool EmailValidation(string Email);
        bool passcodeConfirmation(RegistrationDTO user);
        public bool userRegistration(RegistrationDTO user);

        string CreateToken(User user);
        User Login(LoginDto user);
        object Profile(int userid);
        ICollection<GetTicketDto> activeTickets(int UserID);
        ICollection<GetTicketDto> expiredTickets(int UserID);
        IEnumerable<SoldTicketDto> GetMyTicketInstances(int UserID, int ticketid);
        bool Buy_Ticket(int UserID, int ticketid, int TicketCount);
    }
}
