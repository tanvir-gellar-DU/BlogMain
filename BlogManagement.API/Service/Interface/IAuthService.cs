using BlogManagement.API.Models.DTOs.Auth;

namespace BlogManagement.API.Service.Interface
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string userId);
    }
}
