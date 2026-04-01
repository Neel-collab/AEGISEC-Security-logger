using System.Net.Http.Json;
using AegisSec.Client.Models;
using Microsoft.JSInterop;

namespace AegisSec.Client.Services
{
    public class SecurityService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;
        private string? _token;

        public SecurityService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _http.Timeout = TimeSpan.FromSeconds(10);
            _js = js;
        }

        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new LoginRequest { Username = username, Password = password });
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        public async Task<bool> VerifyOtpAsync(string username, string otp)
        {
            var response = await _http.PostAsJsonAsync("api/auth/verify", new VerifyRequest { Username = username, Otp = otp });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<VerifyResponse>();
                _token = result?.Token;
                await _js.InvokeVoidAsync("localStorage.setItem", "authToken", _token);
                return true;
            }
            return false;
        }

        public async Task<DashboardStats?> GetStatsAsync() =>
            await GetWithAuthAsync<DashboardStats>("api/dashboard/stats");

        public async Task<List<SecurityLog>?> GetLogsAsync() =>
            await GetWithAuthAsync<List<SecurityLog>>("api/dashboard/logs");

        public async Task<List<Alert>?> GetAlertsAsync() =>
            await GetWithAuthAsync<List<Alert>>("api/dashboard/alerts");

        private async Task<T?> GetWithAuthAsync<T>(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(_token))
                {
                    _token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
                }

                if (string.IsNullOrEmpty(_token)) return default;

                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _token);

                var response = await _http.SendAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    await LogoutAsync();
                    return default;
                }

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
            }
            return default;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            if (string.IsNullOrEmpty(_token))
            {
                _token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            }
            return !string.IsNullOrEmpty(_token);
        }

        public async Task LogoutAsync()
        {
            _token = null;
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }
    }
}
