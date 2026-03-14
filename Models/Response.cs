namespace StarBord.Models
{
    public class Response
    {
        public Guid Id { get; set; }
        public Guid ReviewId { get; set; }
        public string ResponseText { get; set; }
        public DateTime ResponseAt { get; set; }
        public Guid RespondedBy { get; set; }
        public Review Review { get; set; }
    }
}
