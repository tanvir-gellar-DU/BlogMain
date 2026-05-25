using System.ComponentModel.DataAnnotations;

namespace BlogManagement.API.Models.DTOs.Ai
{
    public class AiEnhanceRequest
    {
        [Required]
        public string Content { get; set; } = string.Empty;

        public string? Tone { get; set; }

        public bool ImproveGrammar { get; set; } = true;
        public bool ImproveClarity { get; set; } = true;
        public bool ImproveFlow { get; set; } = true;
    }
}
