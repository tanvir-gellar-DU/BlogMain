using BlogManagement.API.Models.Common;
using BlogManagement.API.Models.DTOs.Blog;
using BlogManagement.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogManagement.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPublished([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            (page, pageSize) = NormalizePagination(page, pageSize);
            var blogs = await _blogService.GetPublishedAsync(page, pageSize);
            var total = await _blogService.GetPublishedCountAsync();
            return Ok(ApiResponse<object>.Ok(new { blogs, total, page, pageSize }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var blog = await _blogService.GetByIdAsync(id);
            if (blog == null)
                return NotFound(ApiResponse<BlogResponse>.Fail("Blog not found."));

            return Ok(ApiResponse<BlogResponse>.Ok(blog));
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            (page, pageSize) = NormalizePagination(page, pageSize);
            var blogs = await _blogService.GetByCategoryAsync(categoryId, page, pageSize);
            var total = await _blogService.GetPublishedCountByCategoryAsync(categoryId);
            return Ok(ApiResponse<object>.Ok(new { blogs, total, page, pageSize }));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(ApiResponse<object>.Fail("Search query is required."));

            (page, pageSize) = NormalizePagination(page, pageSize);
            query = query.Trim();
            var blogs = await _blogService.SearchAsync(query, page, pageSize);
            var total = await _blogService.GetPublishedSearchCountAsync(query);
            return Ok(ApiResponse<object>.Ok(new { blogs, total, page, pageSize }));
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyBlogs()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var blogs = await _blogService.GetByUserIdAsync(userId);
            return Ok(ApiResponse<IEnumerable<BlogResponse>>.Ok(blogs));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBlogRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var blog = await _blogService.CreateAsync(request, userId);
                return CreatedAtAction(nameof(GetById), new { id = blog.Id }, ApiResponse<BlogResponse>.Ok(blog, "Blog created successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<BlogResponse>.Fail(ex.Message));
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBlogRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var blog = await _blogService.UpdateAsync(id, request, userId);
                return Ok(ApiResponse<BlogResponse>.Ok(blog, "Blog updated successfully."));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<BlogResponse>.Fail("Blog not found."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<BlogResponse>.Fail(ex.Message));
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await _blogService.DeleteAsync(id, userId);
                return Ok(ApiResponse.Ok("Blog deleted successfully."));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail("Blog not found."));
            }
        }

        private static (int Page, int PageSize) NormalizePagination(int page, int pageSize)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);
            return (page, pageSize);
        }
    }
}
