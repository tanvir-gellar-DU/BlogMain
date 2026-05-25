using BlogManagement.API.Models.DTOs.Blog;

namespace BlogManagement.API.Service.Interface
{
    public interface IBlogService
    {
        Task<BlogResponse?> GetByIdAsync(int id);
        Task<IEnumerable<BlogResponse>> GetAllAsync();
        Task<IEnumerable<BlogResponse>> GetByUserIdAsync(string userId);
        Task<IEnumerable<BlogResponse>> GetPublishedAsync(int page = 1, int pageSize = 10);
        Task<IEnumerable<BlogResponse>> GetByCategoryAsync(int categoryId, int page = 1, int pageSize = 10);
        Task<IEnumerable<BlogResponse>> SearchAsync(string query, int page = 1, int pageSize = 10);
        Task<BlogResponse> CreateAsync(CreateBlogRequest request, string userId);
        Task<BlogResponse> UpdateAsync(int id, UpdateBlogRequest request, string userId);
        Task DeleteAsync(int id, string userId);
        Task<int> GetCountAsync();
        Task<int> GetPublishedCountAsync();
        Task<int> GetPublishedCountByCategoryAsync(int categoryId);
        Task<int> GetPublishedSearchCountAsync(string query);
        Task<int> GetCountByUserAsync(string userId);
    }
}
