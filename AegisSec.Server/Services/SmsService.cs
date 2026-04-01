using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Microsoft.Extensions.Configuration;

namespace AegisSec.Server.Services
{
    public class SmsService
    {
        private readonly IConfiguration _config;

        public SmsService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendSmsAsync(string toNumber, string message)
        {
            try
            {
                var sid = _config["Twilio:AccountSid"];
                var token = _config["Twilio:AuthToken"];
                var fromNum = _config["Twilio:FromNumber"];

                Console.WriteLine("\n[ AEGIS-SEC TELEMETRY ]: INITIATING SMS UPLINK...");
                Console.WriteLine($"[ TARGET ]: {toNumber}");
                Console.WriteLine($"[ MSG ]: {message}");

                if (string.IsNullOrEmpty(sid) || string.IsNullOrEmpty(token)) 
                {
                    Console.WriteLine("[ ERROR ]: TWILIO CREDENTIALS MISSING IN APPSETTINGS.JSON");
                    return false;
                }

                TwilioClient.Init(sid, token);

                var msg = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(fromNum),
                    to: new Twilio.Types.PhoneNumber(toNumber)
                );

                Console.WriteLine($"[ SUCCESS ]: TWILIO DISPATCH STATUS -> {msg.Status}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n**************************************************");
                Console.WriteLine("[ CRITICAL Twilio DISPATCH FAILURE ]");
                Console.WriteLine($"ERROR: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"INNER: {ex.InnerException.Message}");
                Console.WriteLine("**************************************************\n");
                return false;
            }
        }
    }
}
