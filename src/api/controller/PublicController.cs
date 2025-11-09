using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicController : ControllerBase
    {
        [HttpGet("data")]
        [AllowAnonymous] // This endpoint does not require authentication
        public IActionResult GetPublicData()
        {
            return Ok(new { Message = "This is public data" });
        }
    }
}
