namespace StarBord.Models
{
    public class PlatformToken
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Platform { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Business Business { get; set; }

    }
}
