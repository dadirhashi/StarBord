using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using StarBord.Data;
using StarBord.Models;


namespace StarBord.Services
{
    public class TrustpilotService : ITrustpilotService
    {
        private readonly StarBordDbContext _db;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<TrustpilotService> _logger;

        private readonly string _baseUrl;
        private readonly string _clientId;
        private readonly string _clientSecret;

        public TrustpilotService(
            StarBordDbContext db,
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<TrustpilotService> logger)
        {
            _db = db;
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;

            _baseUrl = config["Trustpilot:BaseUrl"]
                ?? throw new InvalidOperationException("Trustpilot:BaseUrl not configured");
            _clientId = config["Trustpilot:ClientId"]
                ?? throw new InvalidOperationException("Trustpilot:ClientId not configured");
            _clientSecret = config["Trustpilot:ClientSecret"]
                ?? throw new InvalidOperationException("Trustpilot:ClientSecret not configured");
        }

        public string BuildAuthorizationUrl(Guid businessId)
        {
            var redirectUri = _config["Trustpilot:RedirectUri"]
                ?? throw new InvalidOperationException("Trustpilot:RedirectUri not configured");

            // NOTE: using businessId directly as state is simplified.
            // TODO before production: sign/encrypt state to prevent CSRF.
            var query = new Dictionary<string, string>
            {
                ["client_id"] = _clientId,
                ["redirect_uri"] = redirectUri,
                ["response_type"] = "code",
                ["state"] = businessId.ToString()
            };

            var qs = string.Join("&", query.Select(kv =>
                $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
            return $"{_baseUrl}/oauth/authorize?{qs}";
        }

        public async Task HandleOAuthCallbackAsync(Guid businessId, string code)
        {
            var redirectUri = _config["Trustpilot:RedirectUri"]
                ?? throw new InvalidOperationException("Trustpilot:RedirectUri not configured");

            var http = _httpClientFactory.CreateClient();
            var basicAuth = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", basicAuth);

            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = redirectUri
            });

            var response = await http.PostAsync(
                $"{_baseUrl}/oauth/oauth-business-users-for-applications/accesstoken",
                form);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError("Trustpilot token exchange failed: {Status} {Body}",
                    response.StatusCode, body);
                throw new InvalidOperationException("Failed to exchange code for token");
            }

            var tokenResp = await response.Content.ReadFromJsonAsync<TrustpilotTokenResponse>()
                ?? throw new InvalidOperationException("Invalid token response from Trustpilot");

            var existing = await _db.PlatformTokens
                .FirstOrDefaultAsync(t => t.BusinessId == businessId && t.Platform == "Trustpilot");

            if (existing != null)
            {
                existing.AccessToken = tokenResp.AccessToken;
                existing.RefreshToken = tokenResp.RefreshToken;
                existing.ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResp.ExpiresIn);
            }
            else
            {
                _db.PlatformTokens.Add(new PlatformToken
                {
                    Id = Guid.NewGuid(),
                    BusinessId = businessId,
                    Platform = "Trustpilot",
                    AccessToken = tokenResp.AccessToken,
                    RefreshToken = tokenResp.RefreshToken,
                    ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResp.ExpiresIn),
                    ExternaalBussinessId = ""  // populated later when we fetch business unit info
                });
            }

            await _db.SaveChangesAsync();
            _logger.LogInformation("Saved Trustpilot token for business {BusinessId}", businessId);
        }

        public async Task<int> SyncReviewsAsync(Guid businessId)
        {
            // TODO implement
            await Task.CompletedTask;
            return 0;
        }

        public async Task PostReplyAsync(Guid businessId, string externalReviewId, string message)
        {
            // TODO implement
            await Task.CompletedTask;
        }

        public async Task<string> EnsureValidAccessTokenAsync(Guid businessId)
        {
            // TODO implement token refresh logic
            await Task.CompletedTask;
            return "";
        }

        private class TrustpilotTokenResponse
        {
            [JsonPropertyName("access_token")] public string AccessToken { get; set; } = "";
            [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; } = "";
            [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; }
            [JsonPropertyName("token_type")] public string TokenType { get; set; } = "";
        }
    }
}
