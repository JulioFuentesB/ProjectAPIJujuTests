namespace Business.Common.DTOs.Post
{
    public class PostCreateDto : IPostDto
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public int Type { get; set; }
        public string Category { get; set; }
        public int CustomerId { get; set; }
    }

    public interface IPostDto
    {
        string Body { get; set; }
        int Type { get; set; }
        string Category { get; set; }
    }
}
