
using BudgetPlanner.Api.DTOs.Categories;

namespace BudgetPlanner.Api.Services.Interfaces;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync(int userId);

    Task<CategoryResponseDto?> GetByIdAsync(
        int categoryId,
        int userId);

    Task<CategoryResponseDto> CreateAsync(
        CreateCategoryDto dto,
        int userId);

    Task<CategoryResponseDto?> UpdateAsync(
        int categoryId,
        UpdateCategoryDto dto,
        int userId);

    Task<bool> DeleteAsync(int categoryId, int userId);
}
