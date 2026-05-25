using System.ComponentModel.DataAnnotations;

namespace BlogManagement.API.Models.Domain
{
    public class Comment
    {
        public int Id { get; set; }

        [Required, StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } = false;

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public int BlogId { get; set; }
        public Blog Blog { get; set; } = null!;

        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
