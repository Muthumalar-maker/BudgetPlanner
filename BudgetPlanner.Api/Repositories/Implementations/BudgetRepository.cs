using BudgetPlanner.Api.Data;
using BudgetPlanner.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanner.Api.Repositories
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly ApplicationDbContext _context;

        public BudgetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all budgets for a user
        public async Task<IEnumerable<Budget>> GetAllAsync(int userId)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Year)
                .ThenByDescending(b => b.Month)
                .ToListAsync();
        }

        // Get budget by ID
        public async Task<Budget?> GetByIdAsync(int budgetId, int userId)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b =>
                    b.BudgetId == budgetId &&
                    b.UserId == userId);
        }

        // Get budgets by month and year
        public async Task<IEnumerable<Budget>> GetByMonthAndYearAsync(
            int userId,
            int month,
            int year)
        {
            return await _context.Budgets
                .Include(b => b.Category)
                .Where(b =>
                    b.UserId == userId &&
                    b.Month == month &&
                    b.Year == year)
                .ToListAsync();
        }

        // Check duplicate budget
        public async Task<bool> ExistsAsync(
            int userId,
            int categoryId,
            int month,
            int year,
            int? excludeBudgetId = null)
        {
            return await _context.Budgets.AnyAsync(b =>
                b.UserId == userId &&
                b.CategoryId == categoryId &&
                b.Month == month &&
                b.Year == year &&
                (!excludeBudgetId.HasValue ||
                 b.BudgetId != excludeBudgetId.Value));
        }

        // Add budget
        public async Task<Budget> AddAsync(Budget budget)
        {
            await _context.Budgets.AddAsync(budget);
            await _context.SaveChangesAsync();

            return budget;
        }

        // Update budget
        public async Task UpdateAsync(Budget budget)
        {
            _context.Budgets.Update(budget);
            await _context.SaveChangesAsync();
        }

        // Delete budget
        public async Task DeleteAsync(Budget budget)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }
    }
}