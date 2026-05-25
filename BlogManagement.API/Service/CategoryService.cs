
using BlogManagement.API.Repositories.Interfaces;
using BlogManagement.API.Models.Domain;

using BlogManagement.API.Service.Interface;

namespace BlogManagement.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<Category?> GetByIdAsync(int id) => await _categoryRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Category>> GetAllAsync() => await _categoryRepository.GetAllAsync();

        public async Task<Category> CreateAsync(string name, string? description)
        {
            name = name.Trim();
            var slug = CreateSlug(name);
            if (await _categoryRepository.GetBySlugAsync(slug) != null)
                throw new InvalidOperationException("Category already exists.");

            var category = new Category
            {
                Name = name,
                Slug = slug,
                Description = description
            };
            return await _categoryRepository.CreateAsync(category);
        }

        public async Task<Category> UpdateAsync(int id, string name, string? description)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            name = name.Trim();
            var slug = CreateSlug(name);
            var existing = await _categoryRepository.GetBySlugAsync(slug);
            if (existing != null && existing.Id != id)
                throw new InvalidOperationException("Category already exists.");

            category.Name = name;
            category.Slug = slug;
            category.Description = description;
            return await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            await _categoryRepository.DeleteAsync(category);
        }

        private static string CreateSlug(string value) => value.ToLower().Replace(" ", "-");
    }
}
