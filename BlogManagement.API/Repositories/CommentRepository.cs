using BlogManagement.API.Data;
using BlogManagement.API.Models.Domain;
using BlogManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.API.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Replies).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Comment?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Replies).ThenInclude(r => r.User)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }

        public async Task<IEnumerable<Comment>> GetByBlogIdAsync(int blogId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Replies).ThenInclude(r => r.User)
                .Where(c => c.BlogId == blogId && c.ParentCommentId == null && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetByUserIdAsync(string userId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Blog)
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment> CreateAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task DeleteAsync(Comment comment)
        {
            comment.IsDeleted = true;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }
    }
}
