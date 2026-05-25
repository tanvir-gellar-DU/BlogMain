using BlogManagement.API.Models.Common;
using BlogManagement.API.Models.Domain;
using BlogManagement.API.Models.DTOs.Comment;
using BlogManagement.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlogManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController :Controller 
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("blog/{blogId}")]
        public async Task<IActionResult> GetByBlog(int blogId)
        {
            var comments = await _commentService.GetByBlogIdAsync(blogId);
            return Ok(ApiResponse<IEnumerable<CommentResponse>>.Ok(comments));
        }

        [Authorize]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyComments()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var comments = await _commentService.GetByUserIdAsync(userId);
            return Ok(ApiResponse<IEnumerable<CommentResponse>>.Ok(comments));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var comment = await _commentService.CreateAsync(request, userId);
                return CreatedAtAction(nameof(GetByBlog), new { blogId = request.BlogId },
                    ApiResponse<CommentResponse>.Ok(comment, "Comment added successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponse<CommentResponse>.Fail(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<CommentResponse>.Fail(ex.Message));
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentRequest request)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var comment = await _commentService.UpdateAsync(id, request.Content, userId);
                return Ok(ApiResponse<CommentResponse>.Ok(comment, "Comment updated successfully."));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<CommentResponse>.Fail("Comment not found."));
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var isAdmin = User.IsInRole("Admin");
                await _commentService.DeleteAsync(id, userId, isAdmin);
                return Ok(ApiResponse.Ok("Comment deleted successfully."));
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse.Fail("Comment not found."));
            }
        }
        public class UpdateCommentRequest
        {
            [System.ComponentModel.DataAnnotations.Required]
            [System.ComponentModel.DataAnnotations.StringLength(1000)]
            public string Content { get; set; } = string.Empty;
        }
    }
}
