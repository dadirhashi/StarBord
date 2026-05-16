using StarBord.Data;
using StarBord.Services.IService;


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
            => throw new NotImplementedException();

        public Task HandleOAuthCallbackAsync(Guid businessId, string code)
            => throw new NotImplementedException();

        public Task<int> SyncReviewsAsync(Guid businessId)
            => throw new NotImplementedException();

        public Task PostReplyAsync(Guid businessId, string externalReviewId, string message)
            => throw new NotImplementedException();

        public Task<string> EnsureValidAccessTokenAsync(Guid businessId)
            => throw new NotImplementedException();
    }
}
