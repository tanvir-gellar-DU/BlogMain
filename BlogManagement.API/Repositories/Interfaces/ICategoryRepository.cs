using BlogManagement.API.Models.Domain;

namespace BlogManagement.API.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(int id);
        Task<Category?> GetBySlugAsync(string slug);
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category> CreateAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task DeleteAsync(Category category);
    }
}
