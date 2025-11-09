using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace YourApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // This makes the entire controller protected
    public class ProtectedController : ControllerBase
    {
        private readonly ILogger<ProtectedController> _logger;

        public ProtectedController(ILogger<ProtectedController> logger)
        {
            _logger = logger;
        }

        // GET: api/protected/profile
        [HttpGet("profile")]
        public IActionResult GetUserProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Auth0 user ID (sub)
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                var name = User.FindFirst(ClaimTypes.Name)?.Value;

                // Alternative: Get Auth0-specific claims
                var auth0UserId = User.FindFirst("sub")?.Value; // Auth0 user ID
                var auth0Email = User.FindFirst("email")?.Value;
                var emailVerified = User.FindFirst("email_verified")?.Value;

                var profile = new
                {
                    UserId = userId ?? auth0UserId,
                    Email = email ?? auth0Email,
                    Name = name,
                    EmailVerified = emailVerified == "true",
                    Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
                };

                _logger.LogInformation($"User profile accessed by: {userId}");
                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        // GET: api/protected/data
        [HttpGet("data")]
        public IActionResult GetProtectedData()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var data = new
                {
                    Message = "This is protected data",
                    UserId = userId,
                    Timestamp = DateTime.UtcNow,
                    Data = new[] { "item1", "item2", "item3" }
                };

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting protected data");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        // POST: api/protected/data
        [HttpPost("data")]
        public IActionResult CreateProtectedData([FromBody] CreateDataRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Process the request
                var result = new
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    Data = request.Data,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation($"Data created by user {userId}");
                return CreatedAtAction(nameof(GetProtectedData), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating protected data");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        // PUT: api/protected/data/{id}
        [HttpPut("data/{id}")]
        public IActionResult UpdateProtectedData(string id, [FromBody] UpdateDataRequest request)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Verify user has permission to update this data
                // (Add your business logic here)

                var result = new
                {
                    Id = id,
                    UserId = userId,
                    Data = request.Data,
                    UpdatedAt = DateTime.UtcNow
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating protected data");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        // DELETE: api/protected/data/{id}
        [HttpDelete("data/{id}")]
        public IActionResult DeleteProtectedData(string id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Verify user has permission to delete this data
                // (Add your business logic here)

                _logger.LogInformation($"Data {id} deleted by user {userId}");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting protected data");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }
    }

    // Request models
    public class CreateDataRequest
    {
        public string Data { get; set; } = string.Empty;
    }

    public class UpdateDataRequest
    {
        public string Data { get; set; } = string.Empty;
    }
}