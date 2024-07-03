using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto.Macs;
using System.Security.Cryptography;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using Tickets_selling_App.Services;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class CustomerController : Controller
    {
        private readonly Customer_Interface _customer;
        private readonly Gmail_Interface _Mail;
        public CustomerController(Customer_Interface customer, Gmail_Interface gmail)
        {
            _customer = customer;
            _Mail = gmail;
        }
        [HttpGet("/SendEmails")]
        public async Task<IActionResult> SendingEmails()
        {
            try
            {
                string myemail = "giorgitsereteli541@gmail.com";
                string subject = "Welcome";
                string message = "You have been Succesfully Registered in our App!";
                string QrData = "QrData";

                await _Mail.SendEmailAsync(myemail, subject, message, QrData); 

                return Ok("Registration Successful");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong: " + ex.Message);
            }
        }


        [HttpPost("/Registration")]
        public  IActionResult Registration(User customer)
        {
            try
            {
                if (customer != null)
                {
                   _customer.Registration(customer);
                }
                return Ok("registration Successfull");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong");
            }
        }
        [HttpGet("/Get Users")]
        public IActionResult AllUsers ()
        {
           try
            {
                var Customer = _customer.AllCustomers();
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
