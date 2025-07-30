namespace Business.Common.DTOs.Post
{
    public interface IPostDto
    {
        string Body { get; set; }
        int Type { get; set; }
        string Category { get; set; }
    }
}
