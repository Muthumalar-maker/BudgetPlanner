
using BudgetPlanner.Api.Models;

namespace BudgetPlanner.Api.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllAsync(int userId);

        Task<Transaction?> GetByIdAsync(int transactionId, int userId);

        Task AddAsync(Transaction transaction);

        Task UpdateAsync(Transaction transaction);

        Task DeleteAsync(Transaction transaction);
    }
}
