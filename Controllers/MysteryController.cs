
using Microsoft.AspNetCore.Mvc;

namespace StorageManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MysteryController : ControllerBase
    {
        [HttpGet("egg")]
        public IActionResult GetMysteryEgg()
        {
            return Ok(new { message = "Congratulations! You have found the ultra mystery easter egg!" });
        }
    }
}
