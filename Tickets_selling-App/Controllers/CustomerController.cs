using Microsoft.AspNetCore.Mvc;

namespace Tickets_selling_App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {
        [HttpGet]
        public IActionResult GetCustomer()
        {
            return View();
        }
    }
}
