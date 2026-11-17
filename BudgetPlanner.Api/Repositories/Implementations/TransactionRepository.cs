
using BudgetPlanner.Api.Data;
using BudgetPlanner.Api.Models;
using BudgetPlanner.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BudgetPlanner.Api.Repositories.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all transactions belonging to the logged-in user
        public async Task<List<Transaction>> GetAllAsync(int userId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        // Get one transaction belonging to the logged-in user
        public async Task<Transaction?> GetByIdAsync(
            int transactionId, int userId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t =>
                    t.TransactionId == transactionId &&
                    t.UserId == userId);
        }

        // Add transaction
        public async Task AddAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        // Update transaction
        public async Task UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        // Delete transaction
        public async Task DeleteAsync(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }
}
