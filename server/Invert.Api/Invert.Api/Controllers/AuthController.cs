using Invert.Api.Dtos;
using Invert.Api.Entities;
using Invert.Api.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Invert.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<AppUser> _userManager;
        public AuthController(IAuthService authService, UserManager<AppUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {

                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if (user == null) return BadRequest(new { Message = "User not found" });

                var result = await _authService.CheckPasswordAsync(user, loginDto.Password);

                if (!result) return BadRequest(new { Message = "Password is incorrect" });
                //generate token

                var token = await _authService.GenerateJwtTokenAsync(user);

                // var refreshToken = await _authService.GenerateAndStoreRefreshTokenAsync(user.Id);

                return Ok(new UserDto
                {
                    DisplayName = user.UserName,
                    Email = user.Email,
                    Token = token,
                    // RefreshToken = ""

                });

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                if (await _authService.UserExistsAsync(registerDto.UserName))
                    return BadRequest(new { Message = "Username is already taken" });

                if (await _authService.EmailExistsAsync(registerDto.Email))
                    return BadRequest(new { Message = "Email is already registered" });

                var user = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                    EmailConfirmed = true,
                };

                await _authService.CreateUserAsync(user, registerDto.Password);

                // var refreshToken = await _authService.GenerateRefreshTokenAsync(user.Id);
                var token = await _authService.GenerateJwtTokenAsync(user);

                return Ok(new UserDto
                {
                    DisplayName = user.UserName,
                    Email = user.Email,
                    Token = token,
                    RefreshToken = ""
                });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var refreshToken = Request.Headers["RefreshToken"].ToString();

                var result = await _authService.LogoutAsync(userId, refreshToken);
                if (!result)
                {
                    return BadRequest(new { Message = "Logout failed" });
                }
                return Ok(new { Message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

    }
}
