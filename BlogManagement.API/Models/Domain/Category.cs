using System.ComponentModel.DataAnnotations;

namespace BlogManagement.API.Models.Domain
{
    public class Category
    {

        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? Slug { get; set; }

        public DateTime CreatedAt { get; set; }

        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    }
}

