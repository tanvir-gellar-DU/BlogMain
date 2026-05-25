using BlogManagement.API.Models.Domain;

namespace BlogManagement.API.Service.Interface
{
    public interface ICategoryService
    {
        Task<Category?> GetByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> CreateAsync(string name, string? description);
        Task<Category> UpdateAsync(int id, string name, string? description);
        Task DeleteAsync(int id);
    }
}
