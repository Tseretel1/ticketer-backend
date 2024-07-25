using Microsoft.AspNetCore.Mvc;
using Tickets_selling_App.Interfaces;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnauthorisedController : Controller
    {
        private readonly NotauthorisedInterface _notAuth;
        public UnauthorisedController (NotauthorisedInterface notAuth)
        {
            _notAuth = notAuth;
        }

        [HttpGet("/all Tickets")]
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

        [HttpGet("/Popular Events")]
        public IActionResult PopularEvents()
        {
            try
            {
                var Tickets = _notAuth.PopularEvents();

                if (Tickets == null || !Tickets.Any())
                    return NotFound("Event not Found");
                return Ok(Tickets);
            }
            catch (Exception ex)
            {
                return BadRequest($"Something went wrong {ex.Message}");
            }
        }
    }
}
