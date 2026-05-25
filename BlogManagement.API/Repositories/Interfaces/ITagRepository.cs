using BlogManagement.API.Models.Domain;

namespace BlogManagement.API.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag?> GetByIdAsync(int id);
        Task<Tag?> GetByNameAsync(string name);
        Task<Tag?> GetBySlugAsync(string slug);
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag> CreateAsync(Tag tag);
        Task<Tag> UpdateAsync(Tag tag);
        Task DeleteAsync(Tag tag);
    }
}
