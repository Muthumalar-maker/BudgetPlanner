
using BudgetPlanner.Api.Models;

namespace BudgetPlanner.Api.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByIdAsync(int userId);

        Task AddAsync(User user);
    }
}
