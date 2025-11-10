using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Shared;

namespace UserRegistrationApi.Functions;

public class EmailWorker
{
    private readonly ILogger<EmailWorker> _logger;

    public EmailWorker(ILogger<EmailWorker> logger)
    {
        _logger = logger;
    }

    // Trigger: Runs when a message appears in 'user-welcome-emails' queue
    [Function(nameof(EmailWorker))]
    public async Task Run(
        [QueueTrigger("user-welcome-emails", Connection = "AzureWebJobsStorage")] QueueMessage message)
    {
        // Deserialize the message body
        var emailJob = JsonSerializer.Deserialize<EmailQueueMessage>(message.Body);

        _logger.LogInformation($"--- STARTED: Processing email for User ID: {emailJob.UserId} ---");
        _logger.LogInformation($"Dequeue Count: {message.DequeueCount}"); // Useful for retry logic!

        try
        {
            // SIMULATION: Real-world SMTP or SendGrid call would go here.
            // We simulate a 3-second network delay.
            await Task.Delay(3000);

            if (emailJob.EmailTo.Contains("fail"))
            {
                // Simulating an error to see how Queues handle retries automatically!
                throw new Exception("Simulated SMTP server timeout!");
            }

            _logger.LogInformation($"EMAIL SENT: Welcome {emailJob.UserFullName} sent to {emailJob.EmailTo}");
            _logger.LogInformation($"--- FINISHED: Email job complete ---");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"FAILED: Could not send email to {emailJob.EmailTo}. Message will return to queue.");
            // By re-throwing, we tell the runtime the message failed.
            // It will retry (default 5 times) before moving it to a 'poison' queue.
            throw;
        }
    }
}