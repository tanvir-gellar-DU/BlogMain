
using BlogManagement.API.Repositories.Interfaces;
using BlogManagement.API.Models.Domain;
using BlogManagement.API.Models.DTOs.Comment;
using BlogManagement.API.Service.Interface;
using Microsoft.AspNetCore.Identity;

namespace BlogManagement.API.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly UserManager<User> _userManager;

        public CommentService(
            ICommentRepository commentRepository,
            IBlogRepository blogRepository,
            UserManager<User> userManager)
        {
            _commentRepository = commentRepository;
            _blogRepository = blogRepository;
            _userManager = userManager;
        }

        public async Task<CommentResponse?> GetByIdAsync(int id)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            return comment == null ? null : MapToResponse(comment);
        }

        public async Task<IEnumerable<CommentResponse>> GetByBlogIdAsync(int blogId)
        {
            var comments = await _commentRepository.GetByBlogIdAsync(blogId);
            return comments.Select(MapToResponse);
        }

        public async Task<IEnumerable<CommentResponse>> GetByUserIdAsync(string userId)
        {
            var comments = await _commentRepository.GetByUserIdAsync(userId);
            return comments.Select(MapToResponse);
        }

        public async Task<CommentResponse> CreateAsync(CreateCommentRequest request, string userId)
        {
            var blog = await _blogRepository.GetByIdAsync(request.BlogId);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            if (request.ParentCommentId.HasValue)
            {
                var parent = await _commentRepository.GetByIdAsync(request.ParentCommentId.Value);
                if (parent == null || parent.BlogId != request.BlogId)
                    throw new InvalidOperationException("Invalid parent comment.");
            }

            var comment = new Comment
            {
                Content = request.Content,
                BlogId = request.BlogId,
                UserId = userId,
                ParentCommentId = request.ParentCommentId,
                CreatedAt = DateTime.UtcNow
            };

            comment = await _commentRepository.CreateAsync(comment);
            var fullComment = await _commentRepository.GetByIdWithDetailsAsync(comment.Id);
            return MapToResponse(fullComment ?? comment);
        }

        public async Task<CommentResponse> UpdateAsync(int id, string content, string userId)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found.");
            if (comment.UserId != userId)
                throw new UnauthorizedAccessException("You can only edit your own comments.");

            comment.Content = content;
            comment.UpdatedAt = DateTime.UtcNow;
            comment = await _commentRepository.UpdateAsync(comment);
            var fullComment = await _commentRepository.GetByIdWithDetailsAsync(comment.Id);
            return MapToResponse(fullComment ?? comment);
        }

        public async Task DeleteAsync(int id, string userId, bool isAdmin)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found.");

            var blog = await _blogRepository.GetByIdAsync(comment.BlogId);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");

            bool isOwnComment = comment.UserId == userId;
            bool isBlogAuthor = blog.UserId == userId;

            if (!isOwnComment && !isBlogAuthor && !isAdmin)
                throw new UnauthorizedAccessException("You cannot delete this comment.");

            await _commentRepository.DeleteAsync(comment);
        }

        private static CommentResponse MapToResponse(Comment comment)
        {
            return new CommentResponse
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                AuthorName = comment.User != null ? $"{comment.User.FirstName} {comment.User.LastName}" : "Unknown User",
                AuthorId = comment.UserId,
                BlogId = comment.BlogId,
                ParentCommentId = comment.ParentCommentId,
                Replies = comment.Replies?.Where(r => !r.IsDeleted).Select(MapToResponse).ToList() ?? new()
            };
        }
    }
}
