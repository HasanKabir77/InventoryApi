using InventoryApi.Domain.DTOs;
using InventoryApi.Domain.Interfaces;
using InventoryApi.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public AuthController(IUnitOfWork uow, ITokenService tokenService)
        {
            _uow = uow;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // check email/username uniqueness
            var existing = _uow.Users.Query().Any(u => u.Email == dto.Email || u.Username == dto.Username);
            if (existing) return Conflict(new { message = "Email or username already exists." });

            var user = new User { Email = dto.Email, Username = dto.Username };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
            await _uow.Users.AddAsync(user);
            await _uow.SaveChangesAsync();

            return CreatedAtAction(null, new { user.Id, user.Email, user.Username });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _uow.Users.Query().FirstOrDefault(u => u.Email == dto.Email);
            if (user == null) return Unauthorized();

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed) return Unauthorized();

            var token = _tokenService.CreateToken(user);
            return Ok(new { token });
        }
    }

}
