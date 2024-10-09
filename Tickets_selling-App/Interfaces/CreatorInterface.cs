using Microsoft.EntityFrameworkCore;
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
        bool DeleteTicket(int TicketId);
        GetTicketDto MatchingTicket(int ticketid);
        public ICollection<GetTicketDto> GetMyActiveTickets(int AccountID, int pageindex);
        public ICollection<GetTicketDto> GetMyExpiredTickets(int AccountID, int pageindex);

        //Staff Management

        object GetMyProfile(int userid,int Userid);
        ICollection<AccountManagment> GetManagement(int AccountID);
        bool RemoveUser(int userid);



        //Regitser Login
        bool register_as_creator(int userid,RegisterAsCreatorDTO cred);
        bool Creator_Account_Register(string accName, int userid);
        string Creator_Account_Login(int accountid,int userid);
        public ICollection<CreatorAccount> myCreatorAccounts(int userId);

        CreatorAccount createdAccountCredentials(string accName, int creatorid);
        List<Ticket> MostViewed(int id);



        //Qr scanner
        ScanedTicketDTO ScanTicket(string ticketId);

    }
}

