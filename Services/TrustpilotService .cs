using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using StarBord.Data;
using StarBord.Models;
using StarBord.Integrations.Trustpilot;

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

        public async Task<string> EnsureValidAccessTokenAsync(Guid businessId)
        {
            var token = await _db.PlatformTokens
                .FirstOrDefaultAsync(t => t.BusinessId == businessId && t.Platform == "Trustpilot");

            if (token == null)
            {
                throw new InvalidOperationException(
                    $"No Trustpilot connection found for business {businessId}. " +
                    "The business owner needs to connect their Trustpilot account first.");
            }

            // 1-minute safety buffer — refresh BEFORE the token actually expires
            if (token.ExpiresAt > DateTime.UtcNow.AddMinutes(1))
            {
                return token.AccessToken;
            }

            _logger.LogInformation(
                "Trustpilot token for business {BusinessId} expired or near expiry, refreshing",
                businessId);

            var http = _httpClientFactory.CreateClient();
            var basicAuth = Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", basicAuth);

            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = token.RefreshToken
            });

            var response = await http.PostAsync(
                $"{_baseUrl}/oauth/oauth-business-users-for-applications/refresh",
                form);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Trustpilot token refresh failed for business {BusinessId}: {Status} {Body}",
                    businessId, response.StatusCode, body);
                throw new InvalidOperationException(
                    $"Failed to refresh Trustpilot token for business {businessId}");
            }

            var tokenResp = await response.Content.ReadFromJsonAsync<TrustpilotTokenResponse>()
                ?? throw new InvalidOperationException("Invalid refresh response from Trustpilot");

            token.AccessToken = tokenResp.AccessToken;
            token.RefreshToken = tokenResp.RefreshToken;
            token.ExpiresAt = DateTime.UtcNow.AddSeconds(tokenResp.ExpiresIn);

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Refreshed Trustpilot token for business {BusinessId}, new expiry {ExpiresAt}",
                businessId, token.ExpiresAt);

            return token.AccessToken;
        }

        public async Task<int> SyncReviewsAsync(Guid businessId)
        {
            // Get a valid token — refreshes automatically if expired
            var accessToken = await EnsureValidAccessTokenAsync(businessId);

            // Look up the Trustpilot Business Unit ID for this business
            var token = await _db.PlatformTokens
                .FirstAsync(t => t.BusinessId == businessId && t.Platform == "Trustpilot");

            // Fallback for mock testing — in real Trustpilot this would always be populated
            var businessUnitId = string.IsNullOrEmpty(token.ExternaalBussinessId)
                ? "mock-business-unit"
                : token.ExternaalBussinessId;

            // Call Trustpilot's reviews endpoint with the access token
            var http = _httpClientFactory.CreateClient();
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await http.GetAsync(
                $"{_baseUrl}/v1/private/business-units/{businessUnitId}/reviews");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                _logger.LogError(
                    "Failed to fetch Trustpilot reviews for business {BusinessId}: {Status} {Body}",
                    businessId, response.StatusCode, body);
                throw new InvalidOperationException("Failed to fetch reviews from Trustpilot");
            }

            var data = await response.Content.ReadFromJsonAsync<TrustpilotReviewsResponse>()
                ?? throw new InvalidOperationException("Invalid reviews response from Trustpilot");

            // Loop through and save any reviews we haven't seen before
            var importedCount = 0;
            foreach (var tpReview in data.Reviews)
            {
                var alreadyImported = await _db.Reviews.AnyAsync(r =>
                    r.Platform == "Trustpilot" &&
                    r.ExternalReviewId == tpReview.Id);

                if (alreadyImported) continue;

                var review = new Review
                {
                    Id = Guid.NewGuid(),
                    BusinessId = businessId,
                    Platform = "Trustpilot",
                    ExternalReviewId = tpReview.Id,
                    Rating = tpReview.Stars,
                    ReviewText = string.IsNullOrEmpty(tpReview.Title)
                        ? tpReview.Text
                        : $"{tpReview.Title}\n\n{tpReview.Text}",
                    ReviewDate = tpReview.CreatedAt
                };

                _db.Reviews.Add(review);
                importedCount++;
            }

            await _db.SaveChangesAsync();

            _logger.LogInformation(
                "Synced {Count} new Trustpilot reviews for business {BusinessId}",
                importedCount, businessId);

            return importedCount;
        }

        // Still a stub — we'll implement this next, after sync testing
        public Task PostReplyAsync(Guid businessId, string externalReviewId, string message)
            => throw new NotImplementedException();
    }
}