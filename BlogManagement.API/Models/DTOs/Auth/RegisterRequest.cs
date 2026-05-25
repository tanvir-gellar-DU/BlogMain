using System.ComponentModel.DataAnnotations;

namespace BlogManagement.API.Models.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required, MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        public string Role { get; set; } = "Author";
    }
}
