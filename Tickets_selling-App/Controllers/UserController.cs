using Azure;
using Microsoft.AspNetCore.Mvc;
using Tickets_selling_App.Dtos;
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
        private readonly IConfiguration _Configuration;
        public UserController(User_Interface customer, IConfiguration configuration)
        {
            _User = customer;
            _Configuration = configuration;
        }

        [HttpPost("/Registration Validation")]
        public async Task<IActionResult> Registration_Validations(string Email)
        {
            try
            {
                if (User != null)
                {
                    string response = _User.Registration_Validation(Email);
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
        public async Task<IActionResult> Registration(User user ,int passcode)
        {
            try
            {
                if (User != null)
                {
                    string response = _User.Registration(user,passcode);
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
            User User_credentials = _User.Login(model.Email, model.Password);
            if (User_credentials != null)
            {
                string token =_User.CreateToken(User_credentials);
                return Ok(token);
            }
            else
            {
                return BadRequest("Email or Password is incorrect!");
            }
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
    }
}
