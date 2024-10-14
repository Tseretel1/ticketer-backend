using Tickets_selling_App.Dtos.TicketDTO;

namespace Tickets_selling_App.Interfaces
{
    public interface NotauthorisedInterface
    {
        ICollection<GetTicketDto> AllMostPopularTickets();
        ICollection<GetTicketDto> MatchingTicket(int ticketid);
        ICollection<GetTicketDto> MostPopularTickets();
        ICollection<GetTicketDto> GetOtherGenreTickets();
        ICollection<GetTicketDto> UpcomingTickets();
        ICollection<GetTicketDto> getByGenre(string genre);
        ICollection<GetTicketDto> MainFilter(string title);
        public ICollection<GetTicketDto> searchbyTitle(string title);
        bool PlusViewCount (int id);
    }
}
