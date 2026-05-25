using BlogManagement.API.Models.DTOs.Comment;

namespace BlogManagement.API.Service.Interface
{
    public interface ICommentService
    {
        Task<CommentResponse?> GetByIdAsync(int id);
        Task<IEnumerable<CommentResponse>> GetByBlogIdAsync(int blogId);
        Task<IEnumerable<CommentResponse>> GetByUserIdAsync(string userId);
        Task<CommentResponse> CreateAsync(CreateCommentRequest request, string userId);
        Task<CommentResponse> UpdateAsync(int id, string content, string userId);
        Task DeleteAsync(int id, string userId, bool isAdmin);
    }
}
