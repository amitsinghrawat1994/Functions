using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace YourApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        // GET: api/user/settings
        [HttpGet("settings")]
        public IActionResult GetUserSettings()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // In a real app, you'd fetch from database
            var settings = new
            {
                UserId = userId,
                Theme = "light",
                Language = "en",
                NotificationsEnabled = true,
                LastLogin = DateTime.UtcNow.AddDays(-1)
            };

            return Ok(settings);
        }

        // PUT: api/user/settings
        [HttpPut("settings")]
        public IActionResult UpdateUserSettings([FromBody] UserSettingsRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // In a real app, you'd save to database
            var updatedSettings = new
            {
                UserId = userId,
                Theme = request.Theme,
                Language = request.Language,
                NotificationsEnabled = request.NotificationsEnabled,
                UpdatedAt = DateTime.UtcNow
            };

            _logger.LogInformation($"Settings updated for user {userId}");
            return Ok(updatedSettings);
        }
    }

    public class UserSettingsRequest
    {
        public string Theme { get; set; } = "light";
        public string Language { get; set; } = "en";
        public bool NotificationsEnabled { get; set; } = true;
    }
}