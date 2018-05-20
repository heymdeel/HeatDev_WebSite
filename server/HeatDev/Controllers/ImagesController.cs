using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HeatDev.Controllers
{
    [Produces("application/json")]
    [Route("api/images")]
    public class ImagesController : Controller
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public ImagesController(IHostingEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null)
            {
                return BadRequest();
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            string path = Path.Combine(hostingEnvironment.WebRootPath, "images", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return Ok($"{Request.Scheme}://{Request.Host}/images/{fileName}");

        }
    }
}