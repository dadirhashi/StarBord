namespace StarBord.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string ReviewText { get; set; }  
        public int Rating { get; set; }
        public string Platform { get; set; }
        public DateTime RiviewDate { get; set; }
        public string ExternalReviewId { get; set; }
        public Business Business { get; set; }
        public Response Response { get; set; }

    }
}
