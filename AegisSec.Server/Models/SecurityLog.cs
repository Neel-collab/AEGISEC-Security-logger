using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AegisSec.Server.Models
{
    [BsonIgnoreExtraElements]
    public class SecurityLog
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("eventType")]
        public string EventType { get; set; } = string.Empty;

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("severity")]
        public string Severity { get; set; } = "LOW";

        [BsonElement("module")]
        public string Module { get; set; } = "General";

        [BsonElement("description")]
        public string Description { get; set; } = string.Empty;

        [BsonElement("sourceIP")]
        public string SourceIP { get; set; } = string.Empty;

        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
