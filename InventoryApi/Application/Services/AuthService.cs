using InventoryApi.Domain.DTOs;
using InventoryApi.Domain.Interfaces;
using InventoryApi.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace InventoryApi.Application.Services
{
    public class AuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly ITokenService _tokenService;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthService(IUnitOfWork uow, ITokenService tokenService)
        {
            _uow = uow;
            _tokenService = tokenService;
        }

        public async Task<User> RegisterAsync(RegisterDto dto)
        {
            if (_uow.Users.Query().Any(u => u.Email == dto.Email || u.Username == dto.Username))
                throw new Exception("Email or Username already exists");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email
            };
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            await _uow.Users.AddAsync(user);
            await _uow.SaveChangesAsync();

            return user;
        }

        public string Login(LoginDto dto)
        {
            var user = _uow.Users.Query().FirstOrDefault(u => u.Email == dto.Email);
            if (user == null) throw new Exception("Invalid credentials");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid credentials");

            return _tokenService.CreateToken(user);
        }
    }
}
