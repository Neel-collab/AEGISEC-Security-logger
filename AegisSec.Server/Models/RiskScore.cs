using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AegisSec.Server.Models
{
    public class RiskScore
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("score")]
        public int Score { get; set; } = 0;

        [BsonElement("entityId")]
        public string EntityId { get; set; } = string.Empty;

        [BsonElement("factors")]
        public List<string> Factors { get; set; } = new List<string>();

        [BsonElement("lastUpdated")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
