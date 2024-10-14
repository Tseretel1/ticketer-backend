using Microsoft.AspNetCore.Mvc;
using Tickets_selling_App.Dtos.TicketDTO;
using Tickets_selling_App.Interfaces;
using Tickets_selling_App.Migrations;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotauthorisedController : Controller
    {
        private readonly NotauthorisedInterface _notAuth;
        public NotauthorisedController (NotauthorisedInterface notAuth)
        {
            _notAuth = notAuth;
        }

        [HttpGet("/matching-ticket/{ticketId}")]
        public ActionResult<GetTicketDto> GetMatchingTicket(int ticketId)
        {
            var result = _notAuth.MatchingTicket(ticketId);
            if (result == null || !result.Any())
            {
                return NotFound();
            }
            return Ok(result);
        }


        [HttpGet("/ticket-categories{filter}")]
        public IActionResult ticketFilterMain(string filter)
        {
            try
            {
                var Tickets = _notAuth.MainFilter(filter);

                if (Tickets == null || !Tickets.Any())
                    return Ok("Event not Found");
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }


        [HttpGet("/all-popular-tickets")]
        public IActionResult getAllMostPopularTickets()
        {
            try
            {
                var Tickets = _notAuth.AllMostPopularTickets();

                if (Tickets == null || !Tickets.Any())
                    return NotFound("Ticket not Found");
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }

        [HttpGet("/popular-tickets")]
        public IActionResult MostPopularTicketss()
        {
            try
            {
                var Tickets = _notAuth.MostPopularTickets();

                if (Tickets == null || !Tickets.Any())
                    return NotFound("Event not Found");
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }

        [HttpGet("/other-tickets")]
        public IActionResult otherTickets()
        {
            try
            {
                var Tickets = _notAuth.GetOtherGenreTickets();

                if (Tickets == null || !Tickets.Any())
                    return NotFound("Event not Found");
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }




        [HttpGet("/upcoming-tickets")]
        public IActionResult upcomingTickets()
        {
            try
            {
                var Tickets = _notAuth.UpcomingTickets();

                if (Tickets == null || !Tickets.Any())
                    return NotFound("Event not Found");
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }


        [HttpGet("/search-by-title/{title}")]
        public IActionResult searchTickets(string title)
        {
            try
            {
                var Tickets = _notAuth.searchbyTitle(title);

                if (Tickets == null || !Tickets.Any())
                    return NotFound("Event not Found");
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }


        [HttpPatch("/view-count")]
        public IActionResult ViewCount([FromBody] int id )
        {
            try
            {
                bool Response  = _notAuth.PlusViewCount(id);
                if (Response)
                {
                    return Ok("ViewCount +");    
                }
                else
                {
                    return NotFound("Ticket not found");
                }
            }
            catch (Exception ex)
            {
               return BadRequest(ex.Message);
            }
        }
    }
}
