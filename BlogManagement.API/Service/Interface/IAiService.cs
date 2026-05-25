using BlogManagement.API.Models.DTOs.Ai;

namespace BlogManagement.API.Service.Interface
{
    public interface IAiService
    {
        Task<string> EnhanceContentAsync(AiEnhanceRequest request);
        Task<string> ChatAsync(AiChatRequest request);
    }
}
