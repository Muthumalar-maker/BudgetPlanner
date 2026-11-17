
using BudgetPlanner.Api.Data;
using BudgetPlanner.Api.DTOs.Categories;
using BudgetPlanner.Api.Models;
using BudgetPlanner.Api.Repositories.Interfaces;
using BudgetPlanner.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanner.Api.Services.Implementations;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;
    private readonly ApplicationDbContext _context;

    public CategoryService(
        ICategoryRepository repository,
        ApplicationDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync(int userId)
    {
        var categories = await _repository.GetAllAsync(userId);

        return categories.Select(MapToDto).ToList();
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(
        int categoryId,
        int userId)
    {
        var category = await _repository.GetByIdAsync(categoryId, userId);

        return category == null ? null : MapToDto(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(
        CreateCategoryDto dto,
        int userId)
    {
        var categoryName = dto.CategoryName.Trim();
        var type = dto.Type.Trim();

        if (await _repository.ExistsAsync(userId, categoryName, type))
        {
            throw new InvalidOperationException(
                "This category already exists.");
        }

        var category = new Category
        {
            UserId = userId,
            CategoryName = categoryName,
            Type = type,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(category);

        return MapToDto(category);
    }

    public async Task<CategoryResponseDto?> UpdateAsync(
        int categoryId,
        UpdateCategoryDto dto,
        int userId)
    {
        var category = await _repository.GetByIdAsync(categoryId, userId);

        if (category == null)
            return null;

        var categoryName = dto.CategoryName.Trim();
        var type = dto.Type.Trim();

        if (await _repository.ExistsAsync(
            userId, categoryName, type, categoryId))
        {
            throw new InvalidOperationException(
                "This category already exists.");
        }

        category.CategoryName = categoryName;
        category.Type = type;

        await _repository.UpdateAsync(category);

        return MapToDto(category);
    }

    public async Task<bool> DeleteAsync(int categoryId, int userId)
    {
        var category = await _repository.GetByIdAsync(categoryId, userId);

        if (category == null)
            return false;

        var isUsedInTransactions = await _context.Transactions
            .AnyAsync(t => t.CategoryId == categoryId &&
                           t.UserId == userId);

        var isUsedInBudgets = await _context.Budgets
            .AnyAsync(b => b.CategoryId == categoryId &&
                           b.UserId == userId);

        if (isUsedInTransactions || isUsedInBudgets)
        {
            throw new InvalidOperationException(
                "Cannot delete this category because it is being used.");
        }

        await _repository.DeleteAsync(category);

        return true;
    }

    private static CategoryResponseDto MapToDto(Category category)
    {
        return new CategoryResponseDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Type = category.Type,
            CreatedAt = category.CreatedAt
        };
    }
}
