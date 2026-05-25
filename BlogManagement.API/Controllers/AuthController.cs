using BlogManagement.API.Models.Common;
using BlogManagement.API.Models.Domain;
using BlogManagement.API.Models.DTOs.Auth;
using BlogManagement.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BlogManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                return Ok(ApiResponse<AuthResponse>.Ok(result, "Registration successful."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<AuthResponse>.Fail(ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(ApiResponse<AuthResponse>.Ok(result, "Login successful."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<AuthResponse>.Fail(ex.Message));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _authService.RefreshTokenAsync(request.RefreshToken);
                return Ok(ApiResponse<AuthResponse>.Ok(result, "Token refreshed successfully."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponse<AuthResponse>.Fail(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
                await _authService.LogoutAsync(userId);

            return Ok(ApiResponse.Ok("Logged out successfully."));
        }
    }
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }


}

