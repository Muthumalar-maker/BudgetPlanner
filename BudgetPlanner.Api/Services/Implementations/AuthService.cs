
using BudgetPlanner.Api.Authentication;
using BudgetPlanner.Api.DTOs.Auth;
using BudgetPlanner.Api.Models;
using BudgetPlanner.Api.Repositories.Interfaces;
using BudgetPlanner.Api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BudgetPlanner.Api.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtService _jwtService;

        public AuthService(
            IAuthRepository repository,
            IPasswordHasher<User> passwordHasher,
            JwtService jwtService)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto?> RegisterAsync(
            RegisterRequestDto request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var existingUser = await _repository.GetByEmailAsync(email);

            if (existingUser != null)
            {
                return null;
            }

            var user = new User
            {
                FullName = request.FullName.Trim(),
                Email = email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                request.Password);

            await _repository.AddAsync(user);

            return CreateAuthResponse(user);
        }

        public async Task<AuthResponseDto?> LoginAsync(
            LoginRequestDto request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _repository.GetByEmailAsync(email);

            if (user == null || !user.IsActive)
            {
                return null;
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password);

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return CreateAuthResponse(user);
        }

        private AuthResponseDto CreateAuthResponse(User user)
        {
            var tokenData = _jwtService.GenerateToken(user);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Token = tokenData.Token,
                ExpiresAt = tokenData.ExpiresAt
            };
        }
    }
}
