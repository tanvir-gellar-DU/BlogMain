namespace BlogManagement.API.Models.DTOs.Comment
{
    public class CommentResponse
    {

        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string AuthorId { get; set; } = string.Empty;
        public int BlogId { get; set; }
        public int? ParentCommentId { get; set; }
        public List<CommentResponse> Replies { get; set; } = new();
    }
}
