namespace StarBord.Models
{
    public class Business
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }  
        public DateTime CreatedAt { get; set; }
        public User User { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<PlatformToken> PlatformTokens { get; set; }

    }
}
