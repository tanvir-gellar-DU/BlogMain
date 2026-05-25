using BlogManagement.API.Data;
using BlogManagement.API.Models.Domain;
using BlogManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.API.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public TagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<Tag?> GetBySlugAsync(string slug)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags.OrderBy(t => t.Name).ToListAsync();
        }

        public async Task<Tag> CreateAsync(Tag tag)
        {
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task<Tag> UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task DeleteAsync(Tag tag)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }
    }
}
