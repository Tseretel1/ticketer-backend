using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Interfaces;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly Admin_Interface _admin;
        public AdminController(Admin_Interface admin)
        {
            _admin = admin;
        }
        [HttpPost("/Add New Tickets")]
        [Authorize(Policy = "CreatorOnly")]
        public IActionResult AddTicket(CreateTicketDto ticket)
        {
            try
            {
                string Response = "";
                if (ticket == null)
                {
                    return BadRequest("Ticket does not exist");
                }

                if (ticket.Activation_Date <= ticket.Expiration_Date)
                {
                    return BadRequest(new {message = "Activation date must be earlier than expiration date" });
                }
                else
                {
                    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    int UserID =  Convert.ToInt32(userId);
                    if (userId != null) {
                        string response = _admin.AddTicket(ticket,UserID);
                        Response = response;
                    }
                    else
                    {
                        return Ok("userid is null");
                    }
                
                }
                return Ok(new { message = Response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("/DeleteTickets/{id}")]
        public IActionResult Delete_Ticket([FromRoute] int id)
        {
            try
            {
                _admin.DeleteTicket(id);
                return Ok("Ticket Successfully Deleted!");
            }
            catch (Exception ex)
            {
                return BadRequest("Something went wrong! " + ex.Message);
            }
        }
    }
}
