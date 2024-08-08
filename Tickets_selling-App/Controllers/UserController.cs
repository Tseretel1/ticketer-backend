using Azure;
using Microsoft.AspNetCore.Mvc;
using Tickets_selling_App.Dtos;
using Tickets_selling_App.Dtos.User;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using Tickets_selling_App.User_Side_Response;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserController : Controller
    {
        private readonly User_Interface _User;
        public UserController(User_Interface customer)
        { 
            _User = customer;
        }

        [HttpPost("/reset password")]
        public async Task<ActionResult> Reset_Password(string Mail)
        {
            string Response = _User.Password_Restoration(Mail);
            return Ok(Response);
        }
        [HttpPost("/Change password")]
        public async Task<ActionResult> Change_Password(string Mail,string password,int passcode)
        {
            string Response = _User.Changing_Password(Mail, password, passcode);
            return Ok(Response);
        }


        [HttpGet("/Get Users")]
        public IActionResult AllUsers ()
        {
           try
            {
                var Customer = _User.AllCustomers();
                if (Customer == null || !Customer.Any())
                    return NotFound("User not Found");
                return Ok(Customer);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }


        //________________________Login Registration servicres

        [HttpPost("/Registration Validation")]
        public async Task<IActionResult> Registration_Validations([FromBody] string email)
        {
            try
            {
                if (email != null)
                {
                    string response = _User.Email_Validation(email);
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
                    string response = _User.Registration(request.User, request.Passcode);
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
            User User_credentials = _User.Login(model);
            if (User_credentials != null)
            {
                string token = _User.CreateToken(User_credentials);
                var ReturToken = new Client_Response
                {
                    Message = token,
                    Success = true
                };
                return Ok(ReturToken);
            }
            else
            {
                var incorrect = new Client_Response
                {
                    Message = "Email or Password is incorrect!",
                };
                return NotFound(incorrect);
            }
        }
    }
}
