using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace submit_feedback_http_trigger.Models;

public class Feedback
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Message { get; set; }

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
