using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface Login_Registration_Interface
    {
        string Registration_Validation(string Email);
        string Registration(User user, int passcode);
        string CreateToken(User user);
        User Login(string Email, string Password);
    }
}
