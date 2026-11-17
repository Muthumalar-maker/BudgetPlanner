
using System.Security.Claims;
using BudgetPlanner.Api.DTOs.Auth;
using BudgetPlanner.Api.Repositories.Interfaces;
using BudgetPlanner.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetPlanner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IAuthRepository _repository;

        public AuthController(
            IAuthService authService,
            IAuthRepository repository)
        {
            _authService = authService;
            _repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequestDto request)
        {
            var result = await _authService.RegisterAsync(request);

            if (result == null)
            {
                return Conflict(new
                {
                    message = "Email address is already registered."
                });
            }

            return CreatedAtAction(
                nameof(Me),
                new { },
                result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var user = await _repository.GetByIdAsync(userId);

            if (user == null || !user.IsActive)
            {
                return Unauthorized();
            }

            return Ok(new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.CreatedAt
            });
        }
    }
}
