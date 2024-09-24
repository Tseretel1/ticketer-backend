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

        [HttpGet("/all-tickets")]
        public IActionResult GetAlltickets()
        {
            try
            {
                var Tickets = _notAuth.GetAll_Tickets();

                if (Tickets == null || !Tickets.Any())
                    return NotFound("Ticket not Found");
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
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



        [HttpGet("/popular-events")]
        public IActionResult PopularEventsCover()
        {
            try
            {
                var Tickets = _notAuth.PopularEventsCover();

                if (Tickets == null || !Tickets.Any())
                    return NotFound("Event not Found");
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

        [HttpGet("/theater-tickets")]
        public IActionResult theaterTickets()
        {
            try
            {
                var Tickets = _notAuth.TheaterTickets();

                if (Tickets == null || !Tickets.Any())
                    return NotFound("Event not Found");
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }

        [HttpGet("/filter-by-category/{categoryName}")]
        public IActionResult categoryFilter(string categoryName)
        {
            try
            {
                var Tickets = _notAuth.getbyCategories(categoryName);
                if (Tickets == null || !Tickets.Any())
                    return NotFound("Event not Found");
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }
        [HttpGet("/search-by-title/{categoryName}")]
        public IActionResult searchTickets(string categoryName)
        {
            try
            {
                var Tickets = _notAuth.getbyCategories(categoryName);

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
