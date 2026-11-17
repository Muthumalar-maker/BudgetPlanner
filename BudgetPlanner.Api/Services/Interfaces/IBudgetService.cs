using BudgetPlanner.Api.DTOs.Budget;

namespace BudgetPlanner.Api.Services
{
    public interface IBudgetService
    {
        Task<IEnumerable<BudgetResponseDto>> GetAllAsync(int userId);

        Task<BudgetResponseDto?> GetByIdAsync(int budgetId, int userId);

        Task<IEnumerable<BudgetResponseDto>> GetByMonthAndYearAsync(
            int userId,
            int month,
            int year);

        Task<BudgetResponseDto> CreateAsync(
            int userId,
            CreateBudgetDto dto);

        Task<BudgetResponseDto?> UpdateAsync(
            int budgetId,
            int userId,
            UpdateBudgetDto dto);

        Task<bool> DeleteAsync(int budgetId, int userId);
    }
}