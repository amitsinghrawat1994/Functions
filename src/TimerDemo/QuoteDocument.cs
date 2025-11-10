using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TimerDemo;

public class QuoteDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonIgnore] // <-- THIS IS THE FIX
    public string Id { get; set; }

    // This mapping is correct
    [JsonPropertyName("id")]
    [BsonElement("QuoteApiId")]
    public int QuoteApiId { get; set; }

    [JsonPropertyName("quote")]
    [BsonElement("Content")]
    public string Content { get; set; }

    [JsonPropertyName("author")]
    [BsonElement("Author")]
    public string Author { get; set; }

    [BsonElement("FetchedAt")]
    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
}