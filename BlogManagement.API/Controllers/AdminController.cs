using BlogManagement.API.Data;
using BlogManagement.API.Models.Common;
using BlogManagement.API.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlogManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public AdminController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var totalBlogs = await _context.Blogs.CountAsync();
            var totalCategories = await _context.Categories.CountAsync();
            var totalComments = await _context.Comments.CountAsync();

            return Ok(ApiResponse<object>.Ok(new
            {
                totalUsers,
                totalBlogs,
                totalCategories,
                totalComments
            }));
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new
                {
                    id = user.Id,
                    username = user.UserName,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    createdAt = user.CreatedAt,
                    role = roles.FirstOrDefault() ?? "Subscriber"
                });
            }

            return Ok(ApiResponse<IEnumerable<object>>.Ok(userList));
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse.Fail("User not found."));
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == id)
            {
                return BadRequest(ApiResponse.Fail("You cannot delete your own admin account."));
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponse.Fail("Failed to delete user."));
            }

            return Ok(ApiResponse.Ok("User deleted successfully."));
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetComments()
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Blog)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    id = c.Id,
                    content = c.Content,
                    createdAt = c.CreatedAt,
                    userName = c.User != null ? c.User.UserName : "Anonymous",
                    blogTitle = c.Blog != null ? c.Blog.Title : "Unknown Post",
                    blogId = c.BlogId
                })
                .ToListAsync();

            return Ok(ApiResponse<IEnumerable<object>>.Ok(comments));
        }

        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound(ApiResponse.Fail("Comment not found."));
            }

            comment.IsDeleted = true;
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse.Ok("Comment deleted successfully."));
        }
    }
}
