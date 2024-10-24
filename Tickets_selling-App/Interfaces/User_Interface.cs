﻿using Tickets_selling_App.Dtos.Ticket;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Dtos.User;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface User_Interface
    {
        ICollection<UsersDTO> AllCustomers();
        string Password_Restoration(string mail);
        string Changing_Password(string mail, string password,int passcode);
        string Email_Validation(string Email);
        string Registration(RegistrationDTO user, int passcode);
        string CreateToken(User user);
        User Login(LoginDto user);
        object Profile(int userid);
        ICollection<GetTicketDto> GetMyTickets(int UserID);
        IEnumerable<SoldTicketDto> GetMyTicketInstances(int UserID, int ticketid);
        bool Buy_Ticket(int UserID, int ticketid, int TicketCount);
    }
}
