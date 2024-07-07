using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Tickets_selling_App.Dtos;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserController : Controller
    {
        private readonly User_Interface _User;
        private readonly Gmail_Interface _Mail;
        private readonly IConfiguration _Configuration;
        public UserController(User_Interface customer, Gmail_Interface gmail, IConfiguration configuration)
        {
            _User = customer;
            _Mail = gmail;
            _Configuration = configuration;
        }
        [HttpGet("/SendEmails")]
        public async Task<IActionResult> SendingEmails(string Mail)
        {
            try
            {
                string QrData = Mail;
                await _Mail.SendEmailAsync(Mail, QrData);

                return Ok("Successful");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong: " + ex.Message);
            }
        }


        [HttpPost("/Registration")]
        public async Task <IActionResult> Registration(User UserDTO)
        {
            try
            {
                if (UserDTO != null)
                {
                    _User.Registration(UserDTO);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
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
