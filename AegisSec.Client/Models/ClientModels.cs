namespace AegisSec.Client.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class SecurityLog
    {
        public string EventType { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SourceIP { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class Alert
    {
        public string AlertId { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class DashboardStats
    {
        public int TotalLogs { get; set; }
        public int HighThreats { get; set; }
        public int FailedLogins { get; set; }
    }

    public class LoginRequest { public string Username { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
    public class LoginResponse { public string Message { get; set; } = string.Empty; public string Username { get; set; } = string.Empty; public string Target { get; set; } = string.Empty; }
    public class VerifyRequest { public string Username { get; set; } = string.Empty; public string Otp { get; set; } = string.Empty; }
    public class VerifyResponse { public string Token { get; set; } = string.Empty; }
}
