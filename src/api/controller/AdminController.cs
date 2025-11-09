using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace YourApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        // GET: api/admin/users - Only for admins
        [HttpGet("users")]
        [Authorize(Roles = "admin")] // Requires admin role
        public IActionResult GetUsers()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // This endpoint is only accessible to users with 'admin' role
            var users = new[]
            {
                new { Id = "1", Name = "User 1", Email = "user1@example.com" },
                new { Id = "2", Name = "User 2", Email = "user2@example.com" }
            };

            _logger.LogInformation($"Admin {userId} accessed user list");
            return Ok(users);
        }

        // POST: api/admin/users - Only for admins
        [HttpPost("users")]
        [Authorize(Roles = "admin")]
        public IActionResult CreateUser([FromBody] CreateUserRequest request)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = new
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Email = request.Email,
                CreatedBy = adminId,
                CreatedAt = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
    }

    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}