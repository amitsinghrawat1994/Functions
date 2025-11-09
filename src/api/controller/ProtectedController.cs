using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // This requires authentication
    public class ProtectedController : ControllerBase
    {
        [HttpGet("data")]
        public IActionResult GetProtectedData()
        {
            // This endpoint requires a valid JWT token
            var userId = User.FindFirst("sub")?.Value; // User ID from Auth0
            var userEmail = User.FindFirst("email")?.Value; // User email if available

            return Ok(new
            {
                Message = "This is protected data",
                UserId = userId,
                Email = userEmail
            });
        }

        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new { Claims = claims });
        }
    }
}
