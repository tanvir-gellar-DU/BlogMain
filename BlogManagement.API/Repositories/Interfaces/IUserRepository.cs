using BlogManagement.API.Models.Domain;

namespace BlogManagement.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
    }
}
