using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiAuth0.Controllers
{
    [Authorize]
    [Route("test")]
    [ApiController]
    public class Test : Controller
    {
        [HttpGet]
        //[Authorize(Roles = "Response")]
        [Authorize(Policy = "read:cuestion")]
        public IActionResult Scoped()
        {
            return Ok(new
            {
                Message = "Hello from a private endpoint! You need to be authenticated and have a scope of read:messages to see this."
            });
        }
    }
}
