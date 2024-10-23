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
        private readonly UserInterface _User;
        private readonly Tkt_Dbcontext _context;
        public UserController(UserInterface customer,Tkt_Dbcontext tkt_Dbcontext)
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

        //________________________Login Registration services

        [HttpPost("/email-validation/{email}")]
        public IActionResult EmailValidation(string email)
        {
             if (!string.IsNullOrEmpty(email)) 
             {
                 bool response = _User.EmailValidation(email);
                 if (response)
                 {
                     var successMessage = new Client_Response<object>
                     {
                         Success = true,
                     };
                     return Ok(successMessage);
                 }
                 var failMessage = new Client_Response<object>
                 {
                     Success = false,
                 };
                 return Ok(failMessage);
             }
             else
             {
                 return BadRequest("User is null");
             }
        }

        [HttpPost("/passcode-confirmation")]
        public IActionResult passcodeConfirmation(RegistrationDTO user)
        {
            bool response = _User.passcodeConfirmation(user);
            if (response)
            {
                var message = new Client_Response<object>
                {
                    Success = true,
                };
                return Ok(message);
            }
            else
            {
                var message = new Client_Response<object>
                {
                    Success = false,
                };
                return Ok(message);
            }
        }

        [HttpPost("/user-registration")]
        public IActionResult Registration([FromBody] RegistrationDTO user)
        {
            try
            {
                if (user.Email != null && user.password!=null)
                {
                    bool response = _User.userRegistration(user);
                    if (response)
                    {
                        var successMessage = new Client_Response<object>
                        {
                            Message = "You successfully registered!",
                            Success = true,
                        };
                        return Ok(successMessage);
                    }
                    var failMessage = new Client_Response<object>
                    {
                        Message = "something went wrong!",
                        Success = false,
                    };
                    return Ok(failMessage);
                }
                else
                {
                    var failMessage = new Client_Response<object>
                    {
                        Message = "can't register User!",
                        Success = false,
                    };
                    return BadRequest(failMessage);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong: {ex.Message}");
            }
        }

        [HttpPost("/login")]
        public IActionResult Login(LoginDto model)
        {
            var User_credentials = _User.Login(model);
            if (User_credentials != null)
            {
                string token = _User.CreateToken(User_credentials);
                var ReturToken = new Client_Response<object>
                {
                    Message = token,
                    Success = true
                };
                return Ok(ReturToken);
            }
            else
            {
                var incorrect = new Client_Response<object>
                {
                    Message = "Email or Password is incorrect!",
                };
                return NotFound(incorrect);
            }
        }

        //my Ticket 
        [HttpGet("/my-active-tickets")]
        [Authorize(Policy = "EveryRole2")]
        public IActionResult MyctiveTikets()
        {
            try
            {
               var userId = User.FindFirst("UserID")?.Value;
               var response = _User.activeTickets(Convert.ToInt32(userId));
               return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong: {ex.Message}");
            }
        }
        [HttpGet("/my-expired-tickets")]
        [Authorize(Policy = "EveryRole2")]
        public IActionResult MyExpiredTikets()
        {
            try
            {
                var userId = User.FindFirst("UserID")?.Value;
                var response = _User.expiredTickets(Convert.ToInt32(userId));
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
                return BadRequest(new Client_Response<object>
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
                        return Ok(new Client_Response<object>
                        {
                            Message = "You bought the ticket successfully.",
                            Success =true,
                        });;
                    }
                    else
                    {
                        return BadRequest(new Client_Response<object>
                        {
                            Success =true,
                            Message = "Something went wrong!",
                        });
                    }
                }

                return BadRequest(new Client_Response<object>
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
