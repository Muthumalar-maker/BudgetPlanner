
using BudgetPlanner.Api.Data;
using BudgetPlanner.Api.Models;
using BudgetPlanner.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanner.Api.Repositories.Implementations;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    // Get all categories for a particular user
    public async Task<List<Category>> GetAllAsync(int userId)
    {
        return await _context.Categories
            .Where(c => c.UserId == userId)
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
    }

    // Get one category belonging to a particular user
    public async Task<Category?> GetByIdAsync(
        int categoryId,
        int userId)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c =>
                c.CategoryId == categoryId &&
                c.UserId == userId);
    }

    // Check duplicate category name and type
    public async Task<bool> ExistsAsync(
        int userId,
        string categoryName,
        string type,
        int? excludeCategoryId = null)
    {
        return await _context.Categories.AnyAsync(c =>
            c.UserId == userId &&
            c.CategoryName.ToLower() == categoryName.ToLower() &&
            c.Type == type &&
            (!excludeCategoryId.HasValue ||
             c.CategoryId != excludeCategoryId.Value));
    }

    // Add category
    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    // Update category
    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    // Delete category
    public async Task DeleteAsync(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}
