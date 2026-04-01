using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AegisSec.Server.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("password")]
        public string Password { get; set; } = string.Empty; // In a real app, hash this!

        [BsonElement("role")]
        public string Role { get; set; } = "User";

        [BsonElement("status")]
        public string Status { get; set; } = "ACTIVE";

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [BsonElement("failedAttempts")]
        public int FailedAttempts { get; set; } = 0;

        [BsonElement("lastLogin")]
        public DateTime LastLogin { get; set; } = DateTime.UtcNow;
    }
}
