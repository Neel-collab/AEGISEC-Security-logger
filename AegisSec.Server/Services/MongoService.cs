using AegisSec.Server.Models;
using MongoDB.Driver;
using MongoDB.Bson;

namespace AegisSec.Server.Services
{
    public class MongoService
    {
        private readonly IMongoDatabase _database;

        public MongoService(IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("MongoDB:ConnectionString").Value ?? "mongodb://127.0.0.1:27017";
            var databaseName = configuration.GetSection("MongoDB:DatabaseName").Value ?? "aegissec_db";

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("users");
        public IMongoCollection<SecurityLog> SecurityLogs => _database.GetCollection<SecurityLog>("security_logs");
        public IMongoCollection<Alert> Alerts => _database.GetCollection<Alert>("alerts");
        public IMongoCollection<RiskScore> RiskScores => _database.GetCollection<RiskScore>("risk_scores");

        // --- User Logic ---
        public async Task<User?> GetUserAsync(string username) =>
            await Users.Find(u => u.Username == username).FirstOrDefaultAsync();

        public async Task UpdateUserLoginAsync(string username)
        {
            var update = Builders<User>.Update
                .Set(u => u.LastLogin, DateTime.UtcNow)
                .Set(u => u.FailedAttempts, 0);
            await Users.UpdateOneAsync(u => u.Username == username, update);
        }

        public async Task IncrementFailedAttemptsAsync(string username)
        {
            var update = Builders<User>.Update.Inc(u => u.FailedAttempts, 1);
            await Users.UpdateOneAsync(u => u.Username == username, update);
        }

        // --- Log Logic ---
        public async Task InsertLogAsync(SecurityLog log) =>
            await SecurityLogs.InsertOneAsync(log);

        public async Task<List<SecurityLog>> GetLogsAsync(int limit = 50) =>
            await SecurityLogs.Find(_ => true)
                .SortByDescending(l => l.Timestamp)
                .Limit(limit)
                .ToListAsync();

        // --- Alert Logic ---
        public async Task<List<Alert>> GetAlertsAsync() =>
            await Alerts.Find(_ => true)
                .SortByDescending(a => a.CreatedAt)
                .ToListAsync();

        // --- Dashboard Logic ---
        public async Task<DashboardStats> GetStatsAsync()
        {
            return new DashboardStats
            {
                TotalLogs = (int)await SecurityLogs.CountDocumentsAsync(_ => true),
                HighThreats = (int)await SecurityLogs.CountDocumentsAsync(l => l.Severity == "HIGH"),
                FailedLogins = (int)await SecurityLogs.CountDocumentsAsync(l => l.EventType == "FAILED_LOGIN")
            };
        }
    }

    public class DashboardStats
    {
        public int TotalLogs { get; set; }
        public int HighThreats { get; set; }
        public int FailedLogins { get; set; }
    }
}
