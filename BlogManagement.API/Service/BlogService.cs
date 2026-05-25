using BlogManagement.API.Repositories.Interfaces;
using BlogManagement.API.Models.Domain;
using BlogManagement.API.Models.DTOs.Blog;
using BlogManagement.API.Service.Interface;
using Microsoft.AspNetCore.Identity;

namespace BlogManagement.API.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ITagRepository _tagRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<User> _userManager;

        public BlogService(IBlogRepository blogRepository, ITagRepository tagRepository, ICategoryRepository categoryRepository, UserManager<User> userManager)
        {
            _blogRepository = blogRepository;
            _tagRepository = tagRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        public async Task<BlogResponse?> GetByIdAsync(int id)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            return blog == null ? null : MapToResponse(blog);
        }

        public async Task<IEnumerable<BlogResponse>> GetAllAsync()
        {
            var blogs = await _blogRepository.GetAllAsync();
            return blogs.Select(MapToResponse);
        }

        public async Task<IEnumerable<BlogResponse>> GetByUserIdAsync(string userId)
        {
            var blogs = await _blogRepository.GetByUserIdAsync(userId);
            return blogs.Select(MapToResponse);
        }

        public async Task<IEnumerable<BlogResponse>> GetPublishedAsync(int page, int pageSize)
        {
            var blogs = await _blogRepository.GetPublishedAsync(page, pageSize);
            return blogs.Select(MapToResponse);
        }

        public async Task<IEnumerable<BlogResponse>> GetByCategoryAsync(int categoryId, int page, int pageSize)
        {
            var blogs = await _blogRepository.GetByCategoryAsync(categoryId, page, pageSize);
            return blogs.Select(MapToResponse);
        }

        public async Task<IEnumerable<BlogResponse>> SearchAsync(string query, int page, int pageSize)
        {
            var blogs = await _blogRepository.SearchAsync(query, page, pageSize);
            return blogs.Select(MapToResponse);
        }

        public async Task<BlogResponse> CreateAsync(CreateBlogRequest request, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");
            await ValidateBlogRequestAsync(request.CategoryId, request.Status);
            var status = NormalizeStatus(request.Status);

            var blog = new Blog
            {
                Title = request.Title,
                Content = request.Content,
                Excerpt = request.Excerpt,
                CoverImageUrl = request.CoverImageUrl,
                Status = status,
                CategoryId = request.CategoryId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                PublishedAt = status == "Published" ? DateTime.UtcNow : null
            };

            foreach (var tagName in NormalizeTags(request.Tags))
            {
                var tagSlug = CreateSlug(tagName);
                var tag = await _tagRepository.GetBySlugAsync(tagSlug);
                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = tagName,
                        Slug = tagSlug
                    };
                    tag = await _tagRepository.CreateAsync(tag);
                }
                blog.BlogTags.Add(new BlogTag { Tag = tag });
            }

            blog = await _blogRepository.CreateAsync(blog);
            var fullBlog = await _blogRepository.GetByIdAsync(blog.Id);
            return MapToResponse(fullBlog ?? blog);
        }

        public async Task<BlogResponse> UpdateAsync(int id, UpdateBlogRequest request, string userId)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");
            if (blog.UserId != userId)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Admin")))
                    throw new UnauthorizedAccessException("You can only edit your own blogs.");
            }
            await ValidateBlogRequestAsync(request.CategoryId, request.Status);
            var status = NormalizeStatus(request.Status);

            blog.Title = request.Title;
            blog.Content = request.Content;
            blog.Excerpt = request.Excerpt;
            blog.CoverImageUrl = request.CoverImageUrl;
            blog.CategoryId = request.CategoryId;
            blog.UpdatedAt = DateTime.UtcNow;

            if (status == "Published" && blog.Status != "Published")
                blog.PublishedAt = DateTime.UtcNow;

            blog.Status = status;

            blog.BlogTags.Clear();
            foreach (var tagName in NormalizeTags(request.Tags))
            {
                var tagSlug = CreateSlug(tagName);
                var tag = await _tagRepository.GetBySlugAsync(tagSlug);
                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = tagName,
                        Slug = tagSlug
                    };
                    tag = await _tagRepository.CreateAsync(tag);
                }
                blog.BlogTags.Add(new BlogTag { Tag = tag });
            }

            blog = await _blogRepository.UpdateAsync(blog);
            var fullBlog = await _blogRepository.GetByIdAsync(blog.Id);
            return MapToResponse(fullBlog ?? blog);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var blog = await _blogRepository.GetByIdAsync(id);
            if (blog == null)
                throw new KeyNotFoundException("Blog not found.");
            if (blog.UserId != userId)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || !(await _userManager.IsInRoleAsync(user, "Admin")))
                    throw new UnauthorizedAccessException("You can only delete your own blogs.");
            }

            await _blogRepository.DeleteAsync(blog);
        }

        public async Task<int> GetCountAsync() => await _blogRepository.GetCountAsync();
        public async Task<int> GetPublishedCountAsync() => await _blogRepository.GetPublishedCountAsync();
        public async Task<int> GetPublishedCountByCategoryAsync(int categoryId) => await _blogRepository.GetPublishedCountByCategoryAsync(categoryId);
        public async Task<int> GetPublishedSearchCountAsync(string query) => await _blogRepository.GetPublishedSearchCountAsync(query);
        public async Task<int> GetCountByUserAsync(string userId) => await _blogRepository.GetCountByUserAsync(userId);

        private static BlogResponse MapToResponse(Blog blog)
        {
            return new BlogResponse
            {
                Id = blog.Id,
                Title = blog.Title,
                Content = blog.Content,
                Excerpt = blog.Excerpt,
                CoverImageUrl = blog.CoverImageUrl,
                Status = blog.Status,
                ViewCount = blog.ViewCount,
                CreatedAt = blog.CreatedAt,
                UpdatedAt = blog.UpdatedAt,
                PublishedAt = blog.PublishedAt,
                AuthorName = blog.User != null ? $"{blog.User.FirstName} {blog.User.LastName}" : "Unknown Author",
                AuthorId = blog.UserId,
                CategoryName = blog.Category?.Name ?? "Uncategorized",
                CategoryId = blog.CategoryId,
                Tags = blog.BlogTags?.Select(bt => bt.Tag?.Name).OfType<string>().ToList() ?? new List<string>(),
                CommentCount = blog.Comments?.Count(c => !c.IsDeleted) ?? 0
            };
        }

        private async Task ValidateBlogRequestAsync(int categoryId, string status)
        {
            if (await _categoryRepository.GetByIdAsync(categoryId) == null)
                throw new InvalidOperationException("Category not found.");

            if (!string.Equals(status, "Draft", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(status, "Published", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Blog status must be Draft or Published.");
        }

        private static string NormalizeStatus(string status) =>
            string.Equals(status, "Published", StringComparison.OrdinalIgnoreCase) ? "Published" : "Draft";

        private static IEnumerable<string> NormalizeTags(IEnumerable<string> tags) =>
            tags.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase);

        private static string CreateSlug(string value) => value.ToLower().Replace(" ", "-");
    }
}
