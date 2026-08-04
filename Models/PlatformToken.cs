namespace StarBord.Models
{
    public class PlatformToken
    {
       
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string Platform { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public string ExternaalBussinessId { get; set; }

        public string AuthorBusinessUserId { get; set; }
        public Business Business { get; set; }

    }
}
