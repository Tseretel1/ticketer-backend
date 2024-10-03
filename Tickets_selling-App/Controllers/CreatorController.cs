using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Reflection.Metadata;
using System.Text;
using Tickets_selling_App.Dtos.Creator;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using Tickets_selling_App.User_Side_Response;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreatorController : Controller
    {
        private readonly CreatorInterface _creator;
        public CreatorController(CreatorInterface admin)
        {
            _creator = admin;
        }

        [HttpPost("/register-as-creator")]
        [Authorize(Policy = "UserOnly")]
        public IActionResult register_as_Creator(RegisterAsCreatorDTO cred)
        {
            try
            {
                var userId = User.FindFirst("UserID")?.Value;
                var register = _creator.register_as_creator(Convert.ToInt32(userId), cred);
                if (register)
                {
                    var message = new Client_Response
                    {
                        Message = "Admin will check and verify you!",
                        Success = true,
                    };
                    return Ok(message);
                }
                else
                {
                    var message = new Client_Response
                    {
                        Message = "You are already registered, wait for admin to verify you!",
                        Success = false,
                    };
                    return Ok(message);
                }
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("/creator-account-registration")]
        [Authorize(Policy = "CreatorOnly")]
        public IActionResult Creator_account_Registration(AccountCreationTDO accountName)
        {
            try
            {
                var userId = User.FindFirst("UserID")?.Value;
                var creatoraccount = _creator.Creator_Account_Register(accountName.AccountName, Convert.ToInt32(userId));
                if (creatoraccount)
                {
                    var message = new Client_Response
                    {
                        Message = "You Created Account Successfully",
                        Success = true,
                    };
                    var ImidiateLogin = _creator.createdAccountCredentials(accountName.AccountName, Convert.ToInt32(userId));
                    if (ImidiateLogin != null)
                    {
                        return Ok(ImidiateLogin);
                    }
                    return Ok(message);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }
        [HttpGet("/creator-account-login/{accountid}")]
        public IActionResult Creator_Account_Login(int accountid)
        {
            try
            {
                var userId = User.FindFirst("UserID")?.Value;
                var checkCredentials = _creator.Creator_Account_Login(accountid, Convert.ToInt32(userId));
                if (checkCredentials != null)
                {
                    var ReturToken = new Client_Response
                    {
                        Message = checkCredentials,
                        Success = true,
                    };
                    return Ok(ReturToken);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("/my-creator-account")]
        public IActionResult creatorAccounts()
        {
            try
            {
                var userId = User.FindFirst("UserID")?.Value;
                var checkCredentials = _creator.myCreatorAccounts(Convert.ToInt32(userId));
                if (checkCredentials != null)
                {
                    return Ok(checkCredentials);
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //Management services 
        [HttpGet("active-tickets")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult activeTickets(int pageindex)
        {
            var userId = User.FindFirst("AccountID")?.Value;
            var MyTickets = _creator.GetMyActiveTickets(Convert.ToInt32(userId),pageindex);
            if (MyTickets != null)
            {
                return Ok(MyTickets);
            }
            return null;
        }
        [HttpGet("expired-tickets")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult expiredTickets(int pageindex)
        {
            var userId = User.FindFirst("AccountID")?.Value;
            var MyTickets = _creator.GetMyExpiredTickets(Convert.ToInt32(userId),pageindex);
            if (MyTickets != null)
            {
                return Ok(MyTickets);
            }
            return null;
        }


        [HttpGet("my-profile")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult MyProfile()
        {
            var AccountID = User.FindFirst("AccountID")?.Value;
            var userid = User.FindFirst("UserID")?.Value;
            var MyTickets = _creator.GetMyProfile(Convert.ToInt32(AccountID), Convert.ToInt32(userid));
            if (MyTickets != null)
            {
                return Ok(MyTickets);
            }
            return null;
        }
        [HttpGet("account-managment")]
        [Authorize(Policy = "AccountAdminOnly")]
        public IActionResult AccountManagment()
        {
            var AccountID = User.FindFirst("AccountID")?.Value;
            var MyTickets = _creator.GetManagement(Convert.ToInt32(AccountID));
            if (MyTickets != null)
            {
                return Ok(MyTickets);
            }
            return null;
        }


        [HttpDelete("remove-user-from-account/{userid}")]
        [Authorize(Policy = "AccountAdminOnly")]
        public IActionResult RemoveUser(int userid)
        {
            var MyTickets = _creator.RemoveUser(userid);
            if (MyTickets)
            {
                return Ok(true);
            }
            return Ok(false);
        }



        //Ticket services 
        [HttpPost("add-new-tickets")]
        [Authorize(Policy = "EveryRole")]
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
                   var AccountID = User.FindFirst("AccountID")?.Value;
                   string response = _creator.AddTicket(ticket, Convert.ToInt32(AccountID));
                   Response = response;
                }
                return Ok(new { message = Response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }

        }
        [HttpPut("update-tickets")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult UpdateTicket([FromBody] CreateTicketDto ticket)
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
                    string response = _creator.UpdateTicket(ticket);
                    Response = response;
                }
                return Ok(new { message = Response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }

        }
        [HttpGet("matching-ticket/{ticketId}")]
        [Authorize(Policy = "EveryRole")]
        public ActionResult<GetTicketDto> GetMatchingTicket(int ticketId)
        {
            var result = _creator.MatchingTicket(ticketId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpDelete("delete-tickets/{id}")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult DeleteTicket(int id)
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


        [HttpGet("most-viewed-tickets")]
        public IActionResult MostViewed(int id)
        {
            var tickets = _creator.MostViewed(id);
            if(tickets != null)
            {
                return Ok(tickets);
            }
            return Ok();
        }




        //Qr code Services 
        [HttpGet("scann-ticket/{ticketId}")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult ScannTicket(string ticketId)
        {
            var result = _creator.ScanTicket(ticketId);
            if (result != null)
            {
                return Ok(result);
            }
            return Ok();
        }
    }
}
