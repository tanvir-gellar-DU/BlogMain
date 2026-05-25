using System.ComponentModel.DataAnnotations;

namespace BlogManagement.API.Models.Domain
{
    public class Blog
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? Excerpt { get; set; }

        public string? CoverImageUrl { get; set; }

        public string Status { get; set; } = "Draft";

        public int ViewCount { get; set; } = 0;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();
    }
}
