
using BudgetPlanner.Api.Models;

namespace BudgetPlanner.Api.Repositories.Interfaces;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync(int userId);

    Task<Category?> GetByIdAsync(int categoryId, int userId);

    Task<bool> ExistsAsync(
        int userId,
        string categoryName,
        string type,
        int? excludeCategoryId = null);

    Task AddAsync(Category category);

    Task UpdateAsync(Category category);

    Task DeleteAsync(Category category);
}
