using BlogManagement.API.Models.Domain;

namespace BlogManagement.API.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment?> GetByIdAsync(int id);
        Task<Comment?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId);
        Task<IEnumerable<Comment>> GetByUserIdAsync(string userId);
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment> UpdateAsync(Comment comment);
        Task DeleteAsync(Comment comment);
    }
}
