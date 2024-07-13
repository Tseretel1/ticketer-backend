using Microsoft.AspNetCore.Mvc;
using Tickets_selling_App.Dtos;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using Tickets_selling_App.User_Side_Response;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class Login_registration : Controller
    {
        private readonly Login_Registration_Interface _Login;
        public Login_registration(Login_Registration_Interface customer)
        {
            _Login = customer;
        }

        [HttpPost("/Registration Validation")]
        public async Task<IActionResult> Registration_Validations(string Email)
        {
            try
            {
                if (User != null)
                {
                    string response = _Login.Registration_Validation(Email);
                    var NewMessage = new Client_Response
                    {
                        Message = response,
                    };
                    return Ok(NewMessage);
                }
                else
                {
                    return BadRequest("User is null");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong: {ex.Message}");
            }
        }

        [HttpPost("/Registration")]
        public async Task<IActionResult> Registration(User user, int passcode)
        {
            try
            {
                if (User != null)
                {
                    string response = _Login.Registration(user, passcode);
                    var NewMessage = new Client_Response
                    {
                        Message = response,
                    };
                    return Ok(NewMessage);
                }
                else
                {
                    return BadRequest("User is null");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong: {ex.Message}");
            }
        }


        [HttpPost("/Login")]
        public async Task<ActionResult<string>> Login([FromBody] UserDto model)
        {
            User User_credentials = _Login.Login(model.Email, model.Password);
            if (User_credentials != null)
            {
                string token = _Login.CreateToken(User_credentials);
                return Ok(token);
            }
            else
            {
                return BadRequest("Email or Password is incorrect!");
            }
        }

    }
}
