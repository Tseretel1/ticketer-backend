using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Interfaces;

namespace Tickets_selling_App.Controllers
{
    public class CreatorController : Controller
    {
        private readonly CreatorInterface _creator;
        public CreatorController(CreatorInterface admin)
        {
            _creator = admin;
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
                    var userId = User.FindFirst("UserID")?.Value;
                    string response = _creator.AddTicket(ticket, Convert.ToInt32(userId));
                        Response = response;
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
