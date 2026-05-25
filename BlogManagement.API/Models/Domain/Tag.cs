using System.ComponentModel.DataAnnotations;

namespace BlogManagement.API.Models.Domain
{
    public class Tag
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public string? Slug { get; set; }

        public ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();
    }
}
