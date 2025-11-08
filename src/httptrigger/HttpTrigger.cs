using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using submit_feedback_http_trigger.Models;
using submit_feedback_http_trigger.Services;

namespace Company.Function;

public class HttpTrigger
{
    private readonly IMongoService _mongoService;
    private readonly ILogger<HttpTrigger> _logger;

    public HttpTrigger(IMongoService mongoService, ILogger<HttpTrigger> logger)
    {
        _mongoService = mongoService;
        _logger = logger;
    }

    [Function("SubmitFeedback")]
    // public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    public async Task<ActionResult> SubmitFeedback([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "feedback")] HttpRequest req)

    {
        // return new OkObjectResult("Welcome to Azure Functions!");
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        try
        {
            var feedback = await req.ReadFromJsonAsync<Feedback>();

            if (feedback is null ||
            string.IsNullOrWhiteSpace(feedback.Name) ||
            string.IsNullOrWhiteSpace(feedback.Email) ||
            string.IsNullOrWhiteSpace(feedback.Message))
            {
                var badRequestMessage = "Invalid input: name, email, and message are required.";
                _logger.LogWarning(badRequestMessage);
                //return BadRequest("Invalid input: name, email, and message are required.");
                return new BadRequestObjectResult(badRequestMessage);
            }

            await _mongoService.InsertFeedbackAsync(feedback);

            // return created object response
            return new OkObjectResult(feedback);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback");
            // 3. **RETURN HTTP 500 to the client**
            return new ObjectResult(new
            {
                message = "An unexpected error occurred. Please try again later.",
                correlationId = Guid.NewGuid().ToString() // Include a correlation ID for tracking,
            })
            {
                StatusCode = 500
            };

        }
    }

    [Function("GetFeedback")]
    public async Task<ActionResult> GetFeedback(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "feedback")] HttpRequest req
    )
    {
        try
        {
            var feedbackList = await _mongoService.GetLatestFeedbackAsync(10);
            return new OkObjectResult(feedbackList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting feedback");
            return new ObjectResult(new
            {
                message = "An unexpected error occurred. Please try again later.",
                correlationId = Guid.NewGuid().ToString() // Include a correlation ID for tracking,
            })
            {
                StatusCode = 500
            };
        }
    }

    // Only allow in development or with a function key
    [Function("SeedFeedback")]
    public async Task<ActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "seed")] HttpRequest req)
    {
#if !DEBUG
            // Optional: Block seeding in non-debug (production-like) environments
            var forbidden = req.CreateResponse(HttpStatusCode.Forbidden);
            await forbidden.WriteStringAsync("Seeding is disabled in this environment.");
            return forbidden;
#endif

        try
        {
            var sampleFeedback = new[]
            {
                    new Feedback { Name = "Alice Johnson", Email = "alice@example.com", Message = "Love the new design!" },
                    new Feedback { Name = "Bob Smith", Email = "bob@example.com", Message = "Could use more color options." },
                    new Feedback { Name = "Charlie Brown", Email = "charlie@example.com", Message = "Works great on mobile!" },
                    new Feedback { Name = "Dana White", Email = "dana@example.com", Message = "Please add dark mode." },
                    new Feedback { Name = "Eve Davis", Email = "eve@example.com", Message = "Fast and responsive UI." }
                };

            foreach (var feedback in sampleFeedback)
            {
                await _mongoService.InsertFeedbackAsync(feedback);
            }

            // var response = req.CreateResponse(HttpStatusCode.Created);
            // await response.WriteStringAsync($"{sampleFeedback.Length} feedback entries seeded successfully.");
            // return response;

            return new OkObjectResult($"{sampleFeedback.Length} feedback entries seeded successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding feedback data");
            return new ObjectResult(new
            {
                message = "An unexpected error occurred. Please try again later.",
                correlationId = Guid.NewGuid().ToString() // Include a correlation ID for tracking,
            })
            {
                StatusCode = 500
            };
        }
    }
}