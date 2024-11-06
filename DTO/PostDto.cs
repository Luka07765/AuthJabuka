namespace Jade.DTO
{
    public class PostDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; }
    }
}
