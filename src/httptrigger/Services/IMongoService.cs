using System;
using submit_feedback_http_trigger.Models;

namespace submit_feedback_http_trigger.Services;

public interface IMongoService
{
    Task<List<Feedback>> GetLatestFeedbackAsync(int count = 10);
    Task InsertFeedbackAsync(Feedback feedback);
}
