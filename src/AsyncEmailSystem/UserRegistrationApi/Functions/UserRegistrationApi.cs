using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shared;

namespace UserRegistrationApi.Functions;

public class UserRegistrationApi
{
    private readonly ILogger<UserRegistrationApi> _logger;

    public UserRegistrationApi(ILogger<UserRegistrationApi> logger)
    {
        _logger = logger;
    }

    [Function("RegisterUser")]
    public async Task<RegistrationOutput> Run(
                [HttpTrigger(AuthorizationLevel.Function, "post", Route = "user/register")] HttpRequest req)
    {
        _logger.LogInformation("Received new user registration request.");

        // 1. Read incoming JSON
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonSerializer.Deserialize<RegistrationRequest>(requestBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (data == null || string.IsNullOrEmpty(data.Email))
        {
            // Need to figure out how to return BadRequest cleanly in simple binding scenarios, 
            // but for this pattern, we'll assume happy path or throw.
            throw new ArgumentException("Invalid registration data");
        }

        // 2. Simulate saving to a real database and getting a new ID
        string newUserId = Guid.NewGuid().ToString();
        _logger.LogInformation($"User saved to DB with ID: {newUserId}");

        // 3. Create the message for the background worker
        var queueMessage = new EmailQueueMessage
        {
            UserId = newUserId,
            EmailTo = data.Email,
            UserFullName = data.FullName,
            RegisteredAt = DateTime.UtcNow
        };

        // 4. Return both the HTTP response and the Queue payload
        return new RegistrationOutput
        {
            // This goes to the HTTP caller immediately
            HttpResponse = new OkObjectResult(new { message = "User registered successfully. Confirmation email sent." }),
            // This goes to the Storage Queue to be picked up later
            QueueMessage = queueMessage
        };
    }
}

public class RegistrationOutput
{
    [QueueOutput("user-welcome-emails", Connection = "AzureWebJobsStorage")]
    public EmailQueueMessage QueueMessage { get; set; }
    [HttpResult]
    public IActionResult HttpResponse { get; set; }
}