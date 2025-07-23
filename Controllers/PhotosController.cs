using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;

namespace StorageManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PhotosController : ControllerBase
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public PhotosController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var uploadsFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var imageFiles = Directory.GetFiles(uploadsFolderPath)
                                      .Select(Path.GetFileName)
                                      .ToList();
            return Ok(imageFiles);
        }
    }
}
