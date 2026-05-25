using System.ComponentModel.DataAnnotations;

namespace BlogManagement.API.Models.DTOs.Ai
{
    public class AiChatRequest
    {
        [Required]
        public string Message { get; set; } = string.Empty;

        public string? BlogContent { get; set; }

        public List<ChatHistoryItem> History { get; set; } = new();
    }
}
