using System.Text.Json.Serialization;

namespace StarBord.Integrations.Trustpilot
{
    public class TrustpilotReviewDto
    {
        [JsonPropertyName("id")] public string Id { get; set; } = "";
        [JsonPropertyName("stars")] public int Stars { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; } = "";
        [JsonPropertyName("text")] public string Text { get; set; } = "";
        [JsonPropertyName("createdAt")] public DateTime CreatedAt { get; set; }
    }
}