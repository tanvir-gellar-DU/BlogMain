namespace BlogManagement.API.Models.DTOs.Blog
{
    public class BlogResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? CoverImageUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public List<string> Tags { get; set; } = new();
        public int CommentCount { get; set; }
    }
}
