using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using Tickets_selling_App.User_Side_Response;

namespace Tickets_selling_App.Controllers
{
    public class CreatorController : Controller
    {
        private readonly CreatorInterface _creator;
        public CreatorController(CreatorInterface admin)
        {
            _creator = admin;
        }

        [HttpPost("/Register As creator")]
        [Authorize(Policy = "UserOnly")]

        public IActionResult RegisterAsCreator([FromBody] Creator creator)
        {
            try
            {
                var userId = User.FindFirst("UserID")?.Value;
                string personalid = creator.PersonalID.ToString();
                string number = creator.PhoneNumber.ToString();

                if (personalid.Length > 12) 
                {
                    var message = new Client_Response
                    {
                        Message = "ID is incorrect, enter no more than 12 characters",
                    };
                    return BadRequest(message); 
                }
                else if (number.Length > 9)
                {
                    var message = new Client_Response
                    {
                        Message = "Number is incorrect, enter no more than 9 characters",
                    };
                    return BadRequest(message);
                }
                else
                {
                    var CreatorRegistered = _creator.Register_as_Creator(creator, Convert.ToInt32(userId));
                    if (CreatorRegistered)
                    {
                        var message = new Client_Response
                        {
                            Message = "We will check your Personal information and then Verify you",
                            Success = false,
                        };
                        return Ok(message);
                    }
                    else
                    {
                        var message = new Client_Response
                        {
                            Message = "You are already registered, wait for admin to verify you!",
                            Success = true,
                        };
                        return Ok(message);
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new Client_Response
                {
                    Message = "An error occurred: " + ex.Message,
                };
                return BadRequest(errorMessage); 
            }
        }
        [HttpGet("/CheckCreator")]
        [Authorize(Policy = "UserOnly")]
        public IActionResult CheckCreator()
        {
            var userId = User.FindFirst("UserID")?.Value;
            var Creator = _creator.CheckCreator(Convert.ToInt32(userId));
            if (Creator)
            {
                var message = new Client_Response
                {
                    Success = true,
                };
                return Ok(message);
            }
            var messages = new Client_Response
            {
                Success = false,
            };
            return Ok(messages);
        }

        [HttpPost("/Add New Tickets Creator")]
        [Authorize(Policy = "CreatorOnly")]
        public IActionResult AddTicket([FromBody] CreateTicketDto ticket)
        {
            try
            {
                string Response = "";
                if (ticket == null)
                {
                    return BadRequest("Ticket does not exist");
                }

                if (ticket.Activation_Date >= ticket.Expiration_Date)
                {
                    return BadRequest(new { message = "Activation date must be earlier than expiration date" });
                }
                else
                {
                    if (ticket.TicketCount<=250)
                    {
                        var userId = User.FindFirst("UserID")?.Value;
                        string response = _creator.AddTicket(ticket, Convert.ToInt32(userId));
                        Response = response;
                    }
                    else
                    {
                        Response = "You cant Create more than 250 Ticket at once!";
                    }
                }
                return Ok(new { message = Response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("/My Tickets")]
        [Authorize(Policy = "CreatorOnly")]
        public IActionResult Mytickets()
        {
            var userId = User.FindFirst("UserID")?.Value;
            var MyTickets = _creator.GetMyTickets(Convert.ToInt32(userId));
            if(MyTickets != null)
            {
                return Ok(MyTickets); 
            }
            return null;
        }


        [HttpGet("/My Profile")]
        [Authorize(Policy = "CreatorOnly")]
        public IActionResult MyProfile()
        {
            var userId = User.FindFirst("UserID")?.Value;
            var MyTickets = _creator.GetMyProfile(Convert.ToInt32(userId));
            if (MyTickets != null)
            {
                return Ok(MyTickets);
            }
            return null;
        }


        [HttpDelete("/DeleteTickets Creator")]
        [Authorize(Policy = "CreatorOnly")]
        public IActionResult Delete_Ticket([FromBody] int id)
        {
            try
            {
                _creator.DeleteTicket(id);
                return Ok("Ticket Successfully Deleted!");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong! " + ex.Message);
            }
        }

    }
}
