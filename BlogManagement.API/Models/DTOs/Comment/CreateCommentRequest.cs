using System.ComponentModel.DataAnnotations;

namespace BlogManagement.API.Models.DTOs.Comment
{
    public class CreateCommentRequest
    {
        [Required, StringLength(1000)]
        public string Content { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int BlogId { get; set; }

        public int? ParentCommentId { get; set; }
    }
}
