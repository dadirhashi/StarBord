namespace StarBord.DTOS
{
    public class GetPlatformTokenDto
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Platform { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
