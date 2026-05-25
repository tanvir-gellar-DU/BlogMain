using System.ComponentModel.DataAnnotations;

namespace BlogManagement.API.Models.DTOs.Blog
{
    public class UpdateBlogRequest
    {
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Excerpt { get; set; }

        public string? CoverImageUrl { get; set; }

        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public List<string> Tags { get; set; } = new();

        [MaxLength(20)]
        public string Status { get; set; } = "Draft";
    }
}
