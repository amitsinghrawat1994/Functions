using System;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using submit_feedback_http_trigger.Models;

namespace submit_feedback_http_trigger.Services;

public class MongoService : IMongoService
{
    private readonly IMongoCollection<Feedback> _feedbackCollection;
    public MongoService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _feedbackCollection = database.GetCollection<Feedback>(settings.Value.CollectionName);
    }

    public async Task<List<Feedback>> GetLatestFeedbackAsync(int count = 10)
    {
        return await _feedbackCollection
            .Find(_ => true)
            .SortByDescending(x => x.CreatedAt)
            .Limit(count)
            .ToListAsync();
    }

    public async Task InsertFeedbackAsync(Feedback feedback)
    {
        await _feedbackCollection.InsertOneAsync(feedback);
    }
}
