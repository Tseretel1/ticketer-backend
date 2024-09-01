using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tickets_selling_App.Dtos.User;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using Tickets_selling_App.User_Side_Response;
using static QRCoder.PayloadGenerator;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class UserController : Controller
    {
        private readonly User_Interface _User;
        private readonly Tkt_Dbcontext _context;
        public UserController(User_Interface customer,Tkt_Dbcontext tkt_Dbcontext)
        { 
            _User = customer;
            _context = tkt_Dbcontext;
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

        [HttpGet("/user-profile")]
        [Authorize(Policy = "EveryRole2")]
        public IActionResult MyProfile()
        {
            try
            {
                var userId = User.FindFirst("UserID")?.Value;
                var Customer = _User.Profile(Convert.ToInt32(userId));
                if (Customer == null)
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

        //Buy Ticket 
        [HttpGet("/my-tickets")]
        [Authorize(Policy = "EveryRole2")]
        public IActionResult MyTikets()
        {
            try
            {
               var userId = User.FindFirst("UserID")?.Value;
               var response = _User.GetMyTickets(Convert.ToInt32(userId));
               return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong: {ex.Message}");
            }
        }
        [HttpGet("/my-tickets-instances/{id}")]
        [Authorize(Policy = "EveryRole2")]

        public IActionResult MyTiketInstances([FromRoute] int id)
        {
            try
            {
                var userId = User.FindFirst("UserID")?.Value;
                var response = _User.GetMyTicketInstances(Convert.ToInt32(userId), id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong: {ex.Message}");
            }
        }


        // Buy ticket 

        public class BuyTicketRequest
        {
            public int TicketId { get; set; }
            public int TicketCount { get; set; }
        }
        [HttpPost("/buy-ticket")]
        [Authorize(Policy = "EveryRole2")]
        public IActionResult BuyTicket([FromBody] BuyTicketRequest request)
        {
            if (request.TicketId <= 0)
            {
                return BadRequest(new Client_Response
                {
                    Message = "Invalid ticket ID."
                });
            }

            try
            {
                var ticket = _context.Tickets.FirstOrDefault(x => x.ID == request.TicketId);
                if (ticket != null && ticket.TicketCount >= request.TicketCount)
                {
                    var userId = User.FindFirst("UserID")?.Value;
                    bool success = _User.Buy_Ticket(Convert.ToInt32(userId), request.TicketId, request.TicketCount);

                    if (success)
                    {
                        return Ok(new Client_Response
                        {
                            Message = "You bought the ticket successfully."
                        });
                    }
                    else
                    {
                        return BadRequest(new Client_Response
                        {
                            Message = "Something went wrong!"
                        });
                    }
                }

                return BadRequest(new Client_Response
                {
                    Message = $"Only {ticket?.TicketCount ?? 0} tickets are left!"
                });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}
