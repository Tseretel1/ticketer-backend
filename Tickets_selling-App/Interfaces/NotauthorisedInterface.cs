﻿using Tickets_selling_App.Dtos.TicketDTO;

namespace Tickets_selling_App.Interfaces
{
    public interface NotauthorisedInterface
    {
        ICollection<GetTicketDto> GetAll_Tickets();
        ICollection<GetTicketDto> MatchingTicket(int ticketid);
        ICollection<GetTicketDto> PopularEvents();
        bool PlusViewCount (int id);
    }
}
