using BlogManagement.API.Models.Domain;

namespace BlogManagement.API.Repositories.Interfaces
{
    public interface IBlogRepository
    {
        Task<Blog?> GetByIdAsync(int id);
        Task<IEnumerable<Blog>> GetAllAsync();
        Task<IEnumerable<Blog>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Blog>> GetPublishedAsync(int page, int pageSize);
        Task<IEnumerable<Blog>> GetByCategoryAsync(int categoryId, int page, int pageSize);
        Task<IEnumerable<Blog>> SearchAsync(string query, int page, int pageSize);
        Task<Blog> CreateAsync(Blog blog);
        Task<Blog> UpdateAsync(Blog blog);
        Task DeleteAsync(Blog blog);
        Task<int> GetCountAsync();
        Task<int> GetPublishedCountAsync();
        Task<int> GetPublishedCountByCategoryAsync(int categoryId);
        Task<int> GetPublishedSearchCountAsync(string query);
        Task<int> GetCountByUserAsync(string userId);
    }
}
