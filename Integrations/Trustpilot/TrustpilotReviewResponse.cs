using System.Text.Json.Serialization;

namespace StarBord.Integrations.Trustpilot
{
    public class TrustpilotReviewsResponse
    {
        [JsonPropertyName("reviews")]
        public List<TrustpilotReviewDto> Reviews { get; set; } = new();
    }
}