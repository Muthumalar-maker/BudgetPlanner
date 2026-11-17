
using BudgetPlanner.Api.DTOs.Transactions;

namespace BudgetPlanner.Api.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionResponseDto>> GetAllAsync(int userId);

        Task<TransactionResponseDto?> GetByIdAsync(
            int transactionId, int userId);

        Task<TransactionResponseDto> CreateAsync(
            CreateTransactionDto dto, int userId);

        Task<TransactionResponseDto?> UpdateAsync(
            int transactionId, UpdateTransactionDto dto, int userId);

        Task<bool> DeleteAsync(int transactionId, int userId);
    }
}
