using AegisSec.Server.Models;
using AegisSec.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AegisSec.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly MongoService _mongoService;
        private readonly SmsService _smsService;
        private readonly IConfiguration _config;
        private static readonly Dictionary<string, (string Otp, DateTime Expiry)> _pendingOtp = new();

        public AuthController(MongoService mongoService, SmsService smsService, IConfiguration config)
        {
            _mongoService = mongoService;
            _smsService = smsService;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _mongoService.GetUserAsync(request.Username);

            if (user == null || user.Password != request.Password)
            {
                await _mongoService.IncrementFailedAttemptsAsync(request.Username);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            if (user.Status == "LOCKED")
                return StatusCode(403, new { message = "Account is locked" });

            // Generate OTP
            var otp = new Random().Next(100000, 999999).ToString();
            _pendingOtp[request.Username] = (otp, DateTime.UtcNow.AddMinutes(5));

            Console.WriteLine("\n==================================================");
            Console.WriteLine("          AEGIS-SEC 2FA INTERCEPT");
            Console.WriteLine($"    USER: {request.Username}");
            Console.WriteLine($"    CODE: {otp}");
            Console.WriteLine("==================================================\n");

            // Send actual SMS if phone number exists
            bool smsSent = false;
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                smsSent = await _smsService.SendSmsAsync(user.PhoneNumber, $"AEGIS-SEC: Verification code is {otp}. Valid for 5 minutes.");
            }

            string maskedPhone = !string.IsNullOrEmpty(user.PhoneNumber) 
                ? $"{user.PhoneNumber.Substring(0, 3)}******{user.PhoneNumber.Substring(user.PhoneNumber.Length - 4)}"
                : "UNKNOWN DEVICE";

            return Ok(new 
            { 
                message = smsSent ? "OTP dispatched via SMS" : "SMS DELIVERY FAILURE (Uplink Blocked)", 
                username = request.Username,
                target = maskedPhone
            });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] VerifyRequest request)
        {
            if (!_pendingOtp.TryGetValue(request.Username, out var otpData) || 
                otpData.Otp != request.Otp || 
                otpData.Expiry < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired OTP" });
            }

            _pendingOtp.Remove(request.Username);

            var user = await _mongoService.GetUserAsync(request.Username);
            if (user == null) return Unauthorized();

            await _mongoService.UpdateUserLoginAsync(request.Username);

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Email", user.Email)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryMinutes"])),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginRequest { public string Username { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
    public class VerifyRequest { public string Username { get; set; } = string.Empty; public string Otp { get; set; } = string.Empty; }
}
