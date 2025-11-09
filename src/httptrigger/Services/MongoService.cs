using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenTelemetry.Trace;
using submit_feedback_http_trigger.Models;

namespace submit_feedback_http_trigger.Services;

public class MongoService : IMongoService
{
    private readonly ILogger<MongoService> _logger;
    private readonly ActivitySource _activitySource; // ← Add this

    private readonly IMongoCollection<Feedback> _feedbackCollection;
    public MongoService(IOptions<MongoDbSettings> settings, ILogger<MongoService> logger)
    {
        _logger = logger;
        _activitySource = new ActivitySource("MongoService"); // ← Create activity source

        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _feedbackCollection = database.GetCollection<Feedback>(settings.Value.CollectionName);
    }

    public async Task<List<Feedback>> GetLatestFeedbackAsync(int count = 10)
    {
        using var activity = _activitySource.StartActivity("GetLatestFeedbackAsync", ActivityKind.Internal);

        try
        {
            return await _feedbackCollection
                .Find(_ => true)
                .SortByDescending(x => x.CreatedAt)
                .Limit(count)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.RecordException(ex);
            throw;
        }
    }

    public async Task InsertFeedbackAsync(Feedback feedback)
    {
        using var activity = _activitySource.StartActivity("InsertFeedbackAsync", ActivityKind.Internal);
        try
        {
            await _feedbackCollection.InsertOneAsync(feedback);
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error);
            activity?.RecordException(ex);
            throw;
        }
    }
}
