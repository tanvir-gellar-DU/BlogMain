using BlogManagement.API.Data;


using BlogManagement.API.Models.Domain;
using BlogManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.API.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly AppDbContext _context;

        public BlogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Blog?> GetByIdAsync(int id)
        {
            return await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .Include(b => b.BlogTags).ThenInclude(bt => bt.Tag)
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Blog>> GetAllAsync()
        {
            return await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .Include(b => b.BlogTags).ThenInclude(bt => bt.Tag)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetByUserIdAsync(string userId)
        {
            return await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .Include(b => b.BlogTags).ThenInclude(bt => bt.Tag)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetPublishedAsync(int page, int pageSize)
        {
            return await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .Include(b => b.BlogTags).ThenInclude(bt => bt.Tag)
                .Where(b => b.Status == "Published")
                .OrderByDescending(b => b.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetByCategoryAsync(int categoryId, int page, int pageSize)
        {
            return await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .Include(b => b.BlogTags).ThenInclude(bt => bt.Tag)
                .Where(b => b.CategoryId == categoryId && b.Status == "Published")
                .OrderByDescending(b => b.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Blog>> SearchAsync(string query, int page, int pageSize)
        {
            return await _context.Blogs
                .Include(b => b.User)
                .Include(b => b.Category)
                .Include(b => b.BlogTags).ThenInclude(bt => bt.Tag)
                .Where(b => b.Status == "Published" &&
                    (b.Title.Contains(query) || b.Content.Contains(query) || b.Excerpt != null && b.Excerpt.Contains(query)))
                .OrderByDescending(b => b.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Blog> CreateAsync(Blog blog)
        {
            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<Blog> UpdateAsync(Blog blog)
        {
            _context.Blogs.Update(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task DeleteAsync(Blog blog)
        {
            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCountAsync() => await _context.Blogs.CountAsync();

        public async Task<int> GetPublishedCountAsync() => await _context.Blogs.CountAsync(b => b.Status == "Published");

        public async Task<int> GetPublishedCountByCategoryAsync(int categoryId) =>
            await _context.Blogs.CountAsync(b => b.CategoryId == categoryId && b.Status == "Published");

        public async Task<int> GetPublishedSearchCountAsync(string query) =>
            await _context.Blogs.CountAsync(b => b.Status == "Published" &&
                (b.Title.Contains(query) || b.Content.Contains(query) || b.Excerpt != null && b.Excerpt.Contains(query)));

        public async Task<int> GetCountByUserAsync(string userId) => await _context.Blogs.CountAsync(b => b.UserId == userId);
    }
}
