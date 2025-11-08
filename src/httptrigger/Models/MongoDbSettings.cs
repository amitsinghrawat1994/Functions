using System;

namespace submit_feedback_http_trigger.Models;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "FeedbackDb";
    public string CollectionName { get; set; } = "Feedback";
}