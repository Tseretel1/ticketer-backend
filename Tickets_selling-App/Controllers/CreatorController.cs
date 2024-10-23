using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.SqlServer.Update.Internal;
using System.Reflection.Metadata;
using System.Text;
using Tickets_selling_App.Dtos.Creator;
using Tickets_selling_App.Dtos.Ticket;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Models;
using Tickets_selling_App.User_Side_Response;
using static Tickets_selling_App.Controllers.CreatorController;

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
                    var message = new Client_Response<object>
                    {
                        Message = "Admin will check and verify you!",
                        Success = true,
                    };
                    return Ok(message);
                }
                else
                {
                    var message = new Client_Response<object>
                    {
                        Message = "You are already registered, wait for admin to verify you!",
                        Success = false,
                    };
                    return Ok(message);
                }
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost("/creator-account-registration")]
        [Authorize(Policy = "CreatorOnly")]
        public IActionResult Creator_account_Registration(AccountCreationTDO accountName)
        {
            var userId = User.FindFirst("UserID")?.Value;
            var creatoraccount = _creator.Creator_Account_Register(accountName.AccountName, Convert.ToInt32(userId));
            if (creatoraccount)
            {
                var message = new Client_Response<object>
                {
                    Message = "You Created Account Successfully",
                    Success = true,
                };
                var ImidiateLogin = _creator.createdAccountCredentials(accountName.AccountName, Convert.ToInt32(userId));
                if (ImidiateLogin != null)
                {
                    var credential = new Client_Response<object>
                    {
                        Message = "You Created Account Successfully",
                        Success = true,
                        accountID = ImidiateLogin.Id
                    };
                   return Ok(credential);

                }

                return Ok(message);
            }
            else
            {
                var message = new Client_Response<object>
                {
                    Message = "Account with given Name Already exists!",
                    Success = false,
                };
                return Ok(message);
            }
        }
        [HttpGet("/creator-account-login/{accountid}")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult Creator_Account_Login(int accountid)
        {
            try
            {
                var userId = User.FindFirst("UserID")?.Value;
                var checkCredentials = _creator.Creator_Account_Login(accountid, Convert.ToInt32(userId));
                if (checkCredentials != null)
                {
                    var ReturToken = new Client_Response<object>
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
        [Authorize(Policy = "EveryRole")]
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
        [HttpGet("all-active-tickets")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult GetAllactiveTickets(int pageindex)
        {
            var userId = User.FindFirst("AccountID")?.Value;
            var MyTickets = _creator.GetAllActiveTickets(Convert.ToInt32(userId));
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
            var myProfile = _creator.GetMyProfile(Convert.ToInt32(AccountID), Convert.ToInt32(userid));
            if (myProfile != null)
            {
                return Ok(myProfile);
            }
            return null;
        }

        public class UpdateProfilePhoto
        {
            public string Photo { get; set; }
        }

        [HttpPut("edit-profile-photo")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult editProfilePhoto([FromBody] UpdateProfilePhoto request)
        {
            if (string.IsNullOrWhiteSpace(request.Photo))
            {
                return BadRequest(new { Message = "Name cannot be empty", Success = false });
            }

            var AccountID = User.FindFirst("AccountID")?.Value;
            bool isUpdated = _creator.editProfilePhoto(Convert.ToInt32(AccountID), request.Photo);

            if (isUpdated)
            {
                return Ok(new Client_Response<object>
                {
                    Message = "Update is successful",
                    Success = true
                });
            }

            return BadRequest(new { Message = "Could not update", Success = false });
        }



        public class UpdateProfileRequest
        {
            public string Name { get; set; }
        }
        [HttpPut("edit-profile-name")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult editProfileName([FromBody] UpdateProfileRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(new { Message = "Name cannot be empty", Success = false });
            }

            var AccountID = User.FindFirst("AccountID")?.Value;
            bool isUpdated = _creator.editProfileName(Convert.ToInt32(AccountID), request.Name);

            if (isUpdated)
            {
                return Ok(new Client_Response<object>
                {
                    Message = "Update is successful",
                    Success = true
                });
            }

            return BadRequest(new { Message = "Could not update", Success = false });
        }


        [HttpGet("account-management")]
        [Authorize(Policy = "CreatorOnly")]
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
                var ReturnMessage = new Client_Response<object>
                {
                    Message = Response,
                    Success = true,
                };
                return Ok(ReturnMessage);
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
                var ReturnMessage = new Client_Response<object>   
                {
                    Message = Response,
                    Success = true,
                };
                return Ok(ReturnMessage);
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
                bool isRemoved = _creator.DeleteTicket(id);
                if (isRemoved)
                {
                    var Returmessage = new Client_Response<object>
                    {
                        Message = "Ticket Successfully Deleted!",
                        Success = true,
                    };
                    return Ok(Returmessage);
                }
                var ReturmessageFalse = new Client_Response<object>
                {
                    Message = "Could not find ticket to delete!",
                    Success = true,
                };
                return NotFound(ReturmessageFalse);
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
            var accountid = User.FindFirst("AccountID")?.Value;
            var result = _creator.checkTicketScann(ticketId, Convert.ToInt32(accountid));
            if (result != null)
            {
                return Ok(result);
            }
            return Ok();
        }
        [HttpGet("one-time-scann/{ticketId}")]
        [Authorize(Policy = "EveryRole")]
        public IActionResult ScannTicketOneTime(string ticketId)
        {
            var accountid = User.FindFirst("AccountID")?.Value;
            var result = _creator.oneTimeScann(ticketId, Convert.ToInt32(accountid));
            if (result != null)
            {
                return Ok(result);
            }
            return Ok();
        }
    }
}
