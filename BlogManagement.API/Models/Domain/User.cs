using Microsoft.AspNetCore.Identity;

namespace BlogManagement.API.Models.Domain
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
