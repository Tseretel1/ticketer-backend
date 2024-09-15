using Tickets_selling_App.Dtos.Creator;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface CreatorInterface
    {
        //Ticket Crud
        string AddTicket(CreateTicketDto ticket,int id);
        string UpdateTicket(CreateTicketDto ticket);
        void DeleteTicket(int TicketId);
        GetTicketDto MatchingTicket(int ticketid);


        //Staff Management
        public ICollection<GetTicketDto> GetMyTickets(int UserID);
        object GetMyProfile(int userid,int Userid);
        ICollection<AccountManagment> GetManagement(int AccountID);
        bool RemoveUser(int userid);



        //Regitser Login
        bool register_as_creator(int userid,RegisterAsCreatorDTO cred);
        bool Creator_Account_Register(CreatorAccount acc,int userid);
        string Creator_Account_Login(string username, string passwrod, int userid);        
        List<Ticket> MostViewed(int id);

        //Qr scanner
        ScanedTicketDTO ScanTicket(string ticketId);

    }
}

