
using BudgetPlanner.Api.Data;
using BudgetPlanner.Api.DTOs.Transactions;
using BudgetPlanner.Api.Models;
using BudgetPlanner.Api.Repositories.Interfaces;
using BudgetPlanner.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanner.Api.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;
        private readonly ApplicationDbContext _context;

        public TransactionService(
            ITransactionRepository repository,
            ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        // Get all transactions
        public async Task<List<TransactionResponseDto>> GetAllAsync(
            int userId)
        {
            var transactions = await _repository.GetAllAsync(userId);

            return transactions.Select(MapToDto).ToList();
        }

        // Get transaction by ID
        public async Task<TransactionResponseDto?> GetByIdAsync(
            int transactionId, int userId)
        {
            var transaction = await _repository.GetByIdAsync(
                transactionId, userId);

            return transaction == null ? null : MapToDto(transaction);
        }

        // Create transaction
        public async Task<TransactionResponseDto> CreateAsync(
            CreateTransactionDto dto, int userId)
        {
            var category = await ValidateCategoryAsync(
                dto.CategoryId, dto.Type, userId);

            var transaction = new Transaction
            {
                UserId = userId,
                CategoryId = category.CategoryId,
                Amount = dto.Amount,
                Type = dto.Type,
                Description = dto.Description?.Trim(),
                TransactionDate = dto.TransactionDate!.Value,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(transaction);

            var savedTransaction = await _repository.GetByIdAsync(
                transaction.TransactionId, userId);

            return MapToDto(savedTransaction!);
        }

        // Update transaction
        public async Task<TransactionResponseDto?> UpdateAsync(
            int transactionId,
            UpdateTransactionDto dto,
            int userId)
        {
            var transaction = await _repository.GetByIdAsync(
                transactionId, userId);

            if (transaction == null)
                return null;

            var category = await ValidateCategoryAsync(
                dto.CategoryId, dto.Type, userId);

            transaction.CategoryId = category.CategoryId;
            transaction.Amount = dto.Amount;
            transaction.Type = dto.Type;
            transaction.Description = dto.Description?.Trim();
            transaction.TransactionDate = dto.TransactionDate!.Value;

            await _repository.UpdateAsync(transaction);

            var updatedTransaction = await _repository.GetByIdAsync(
                transactionId, userId);

            return MapToDto(updatedTransaction!);
        }

        // Delete transaction
        public async Task<bool> DeleteAsync(
            int transactionId, int userId)
        {
            var transaction = await _repository.GetByIdAsync(
                transactionId, userId);

            if (transaction == null)
                return false;

            await _repository.DeleteAsync(transaction);

            return true;
        }

        // Validate category ownership and type
        private async Task<Category> ValidateCategoryAsync(
            int categoryId, string transactionType, int userId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == categoryId &&
                    c.UserId == userId);

            if (category == null)
            {
                throw new InvalidOperationException(
                    "Category not found or does not belong to this user.");
            }

            if (!string.Equals(
                category.Type,
                transactionType,
                StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Transaction type must match the category type.");
            }

            return category;
        }

        // Convert Entity to Response DTO
        private static TransactionResponseDto MapToDto(
            Transaction transaction)
        {
            return new TransactionResponseDto
            {
                TransactionId = transaction.TransactionId,
                CategoryId = transaction.CategoryId,
                CategoryName = transaction.Category?.CategoryName
                    ?? string.Empty,
                Amount = transaction.Amount,
                Type = transaction.Type,
                Description = transaction.Description,
                TransactionDate = transaction.TransactionDate,
                CreatedAt = transaction.CreatedAt
            };
        }
    }
}
