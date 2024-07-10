using Tickets_selling_App.Dtos;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface User_Interface
    {
        string Registration_Validation(string Email);
        string Registration(User user,int passcode);
        User Login (string Email, string Password);
        string CreateToken(User user);
        ICollection<EveryUsersDTO> AllCustomers();
        string Password_Restoration(string mail);
        string Changing_Password(string mail, string password,int passcode);
    }
}
