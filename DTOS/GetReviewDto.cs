namespace StarBord.DTOS
{
    public class GetReviewDto
    {
        public Guid Id { get; set; }
        public Guid BusinessId { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public string Platform { get; set; }
        public DateTime ReviewDate { get; set; }
        public string? ExternalReviewId { get; set; }
    }
}
