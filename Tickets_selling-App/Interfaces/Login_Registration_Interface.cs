using Tickets_selling_App.Dtos.User;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Interfaces
{
    public interface Login_Registration_Interface
    {
        string Email_Validation(string Email);
        string Registration(RegistrationDTO user, int passcode);
        string CreateToken(User user);
        User Login(LoginDto user);
    }
}
