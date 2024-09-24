using Tickets_selling_App.Dtos.TicketDTO;

namespace Tickets_selling_App.Interfaces
{
    public interface NotauthorisedInterface
    {
        ICollection<GetTicketDto> GetAll_Tickets();
        ICollection<GetTicketDto> MatchingTicket(int ticketid);
        ICollection<GetTicketDto> PopularEventsCover();
        ICollection<GetTicketDto> MostPopularTickets();
        ICollection<GetTicketDto> UpcomingTickets();
        ICollection<GetTicketDto> TheaterTickets();

        ICollection<GetTicketDto> getbyCategories(string genre);
        public ICollection<GetTicketDto> searchbyTitle(string title);
        bool PlusViewCount (int id);
    }
}
