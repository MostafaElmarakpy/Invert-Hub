using Invert.Api.Dtos;
using Invert.Api.Entities;

namespace Invert.Api.Services.Interface
{
    public interface IAuthService
    {
        Task<AppUser> GetUserByNameAsync(string userName);
        //get user by email
        Task<AppUser> GetUserByEmailAsync(string email);
        Task<bool> UserExistsAsync(string userName);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> CheckPasswordAsync(AppUser user, string password);
        Task CreateUserAsync(AppUser user, string password);
        Task UpdateUserAsync(AppUser user);
        Task<string> GenerateJwtTokenAsync(AppUser user);
        Task<string> GenerateRefreshTokenAsync();
        Task<bool> LogoutAsync(string userId, string RefreshToken);
    }
}
