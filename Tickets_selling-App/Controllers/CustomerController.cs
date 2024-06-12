using Microsoft.AspNetCore.Mvc;
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
        public CustomerController(Customer_Interface customer) 
        {
            _customer = customer;
        }

        [HttpPost("/Registration")]
        public IActionResult Registration(Customer customer)
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
