namespace StarBord.Services
{
    public interface ITrustpilotService
    {
        string BuildAuthorizationUrl(Guid businessId);
        Task HandleOAuthCallbackAsync(Guid businessId, string code);
        Task<int> SyncReviewsAsync(Guid businessId);
        Task PostReplyAsync(Guid businessId, string externalReviewId, string message);
        Task<string> EnsureValidAccessTokenAsync(Guid businessId);
    }
}
