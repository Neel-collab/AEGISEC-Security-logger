using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AegisSec.Server.Models
{
    public class Alert
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("alertId")]
        public string AlertId { get; set; } = string.Empty;

        [BsonElement("severity")]
        public string Severity { get; set; } = "HIGH";

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("assignedTo")]
        public string AssignedTo { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
