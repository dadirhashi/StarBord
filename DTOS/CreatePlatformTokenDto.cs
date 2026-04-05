namespace StarBord.DTOS
{
    public class CreatePlatformTokenDto
    {
        public Guid BusinessId { get; set; }
        public string Platform { get; set; }
        public string AccessToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
