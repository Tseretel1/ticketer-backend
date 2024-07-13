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
    }
}
