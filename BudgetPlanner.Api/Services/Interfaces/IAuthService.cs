
using BudgetPlanner.Api.DTOs.Auth;

namespace BudgetPlanner.Api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(
            RegisterRequestDto request);

        Task<AuthResponseDto?> LoginAsync(
            LoginRequestDto request);
    }
}
