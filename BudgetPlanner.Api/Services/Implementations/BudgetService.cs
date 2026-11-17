using BudgetPlanner.Api.Data;
using BudgetPlanner.Api.DTOs.Budget;
using BudgetPlanner.Api.Models;
using BudgetPlanner.Api.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanner.Api.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepository;
        private readonly ApplicationDbContext _context;

        public BudgetService(
            IBudgetRepository budgetRepository,
            ApplicationDbContext context)
        {
            _budgetRepository = budgetRepository;
            _context = context;
        }

        // Get all budgets
        public async Task<IEnumerable<BudgetResponseDto>> GetAllAsync(int userId)
        {
            var budgets = await _budgetRepository.GetAllAsync(userId);

            return budgets.Select(MapToDto);
        }

        // Get budget by ID
        public async Task<BudgetResponseDto?> GetByIdAsync(
            int budgetId,
            int userId)
        {
            var budget = await _budgetRepository.GetByIdAsync(
                budgetId, userId);

            return budget == null ? null : MapToDto(budget);
        }

        // Get budgets by month and year
        public async Task<IEnumerable<BudgetResponseDto>> GetByMonthAndYearAsync(
            int userId,
            int month,
            int year)
        {
            if (month < 1 || month > 12)
                throw new ArgumentException("Month must be between 1 and 12.");

            if (year < 2000 || year > 2100)
                throw new ArgumentException("Invalid year.");

            var budgets = await _budgetRepository.GetByMonthAndYearAsync(
                userId, month, year);

            return budgets.Select(MapToDto);
        }

        // Create budget
        public async Task<BudgetResponseDto> CreateAsync(
            int userId,
            CreateBudgetDto dto)
        {
            // Check whether category belongs to this user
            var categoryExists = await _context.Categories.AnyAsync(c =>
                c.CategoryId == dto.CategoryId &&
                c.UserId == userId);

            if (!categoryExists)
                throw new ArgumentException(
                    "Category not found for this user.");

            // Check duplicate budget
            var exists = await _budgetRepository.ExistsAsync(
                userId,
                dto.CategoryId,
                dto.Month,
                dto.Year);

            if (exists)
                throw new InvalidOperationException(
                    "A budget already exists for this category and month.");

            var budget = new Budget
            {
                UserId = userId,
                CategoryId = dto.CategoryId,
                MonthlyLimit = dto.MonthlyLimit,
                Month = dto.Month,
                Year = dto.Year,
                CreatedAt = DateTime.UtcNow
            };

            var createdBudget = await _budgetRepository.AddAsync(budget);

            var result = await _budgetRepository.GetByIdAsync(
                createdBudget.BudgetId, userId);

            return MapToDto(result!);
        }

        // Update budget
        public async Task<BudgetResponseDto?> UpdateAsync(
            int budgetId,
            int userId,
            UpdateBudgetDto dto)
        {
            var budget = await _budgetRepository.GetByIdAsync(
                budgetId, userId);

            if (budget == null)
                return null;

            var categoryExists = await _context.Categories.AnyAsync(c =>
                c.CategoryId == dto.CategoryId &&
                c.UserId == userId);

            if (!categoryExists)
                throw new ArgumentException(
                    "Category not found for this user.");

            var exists = await _budgetRepository.ExistsAsync(
                userId,
                dto.CategoryId,
                dto.Month,
                dto.Year,
                budgetId);

            if (exists)
                throw new InvalidOperationException(
                    "A budget already exists for this category and month.");

            budget.CategoryId = dto.CategoryId;
            budget.MonthlyLimit = dto.MonthlyLimit;
            budget.Month = dto.Month;
            budget.Year = dto.Year;

            await _budgetRepository.UpdateAsync(budget);

            var updatedBudget = await _budgetRepository.GetByIdAsync(
                budgetId, userId);

            return updatedBudget == null ? null : MapToDto(updatedBudget);
        }

        // Delete budget
        public async Task<bool> DeleteAsync(int budgetId, int userId)
        {
            var budget = await _budgetRepository.GetByIdAsync(
                budgetId, userId);

            if (budget == null)
                return false;

            await _budgetRepository.DeleteAsync(budget);

            return true;
        }

        // Map Budget model to response DTO
        private static BudgetResponseDto MapToDto(Budget budget)
        {
            return new BudgetResponseDto
            {
                BudgetId = budget.BudgetId,
                UserId = budget.UserId,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category?.CategoryName ?? string.Empty,
                MonthlyLimit = budget.MonthlyLimit,
                Month = budget.Month,
                Year = budget.Year,
                CreatedAt = budget.CreatedAt
            };
        }
    }
}