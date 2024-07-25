using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Tickets_selling_App.Dtos.User;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using Tickets_selling_App.User_Side_Response;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class Login_Register_Controller : Controller
    {
        private readonly Login_Registration_Interface _Login;
        public Login_Register_Controller(Login_Registration_Interface customer)
        {
            _Login = customer;
        }

        [HttpPost("/Registration Validation")]
        public async Task<IActionResult> Registration_Validations([FromBody] string email)
        {
            try
            {
                if (email != null)
                {
                    string response = _Login.Email_Validation(email);
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
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest request)
        {
            try
            {
                if (request.User != null)
                {
                    string response = _Login.Registration(request.User, request.Passcode);
                    var NewMessage = new Client_Response
                    {
                        Message = response,
                    };
                    return Ok(NewMessage);
                }
                else
                {
                    return BadRequest("can't register User does not exist!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong: {ex.Message}");
            }
        }

        public class RegistrationRequest
        {
            public RegistrationDTO User { get; set; }
            public int Passcode { get; set; }
        }


        [HttpPost("/Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto model)
        {
            User User_credentials = _Login.Login(model);
            if (User_credentials != null)
            {
                string token = _Login.CreateToken(User_credentials);
                var ReturToken = new Client_Response
                {
                    Message = token,
                };
                return Ok(ReturToken);
            }
            else
            {
                var ReturToken = new Client_Response
                {
                    Message = "Email or Password is incorrect!",
                };
                return NotFound(ReturToken);
            }
        }

    }
}
