using BudgetPlanner.Api.Models;

namespace BudgetPlanner.Api.Repositories
{
    public interface IBudgetRepository
    {
        Task<IEnumerable<Budget>> GetAllAsync(int userId);

        Task<Budget?> GetByIdAsync(int budgetId, int userId);

        Task<IEnumerable<Budget>> GetByMonthAndYearAsync(
            int userId,
            int month,
            int year);

        Task<bool> ExistsAsync(
            int userId,
            int categoryId,
            int month,
            int year,
            int? excludeBudgetId = null);

        Task<Budget> AddAsync(Budget budget);

        Task UpdateAsync(Budget budget);

        Task DeleteAsync(Budget budget);
    }
}