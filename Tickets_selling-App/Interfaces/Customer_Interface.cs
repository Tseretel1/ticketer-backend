using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface Customer_Interface
    {
        void Registration (User customer);
        ICollection<User> AllCustomers();
    }
}
