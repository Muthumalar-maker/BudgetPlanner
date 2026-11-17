
using System.ComponentModel.DataAnnotations;

namespace BudgetPlanner.Api.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [Range(0.01, 999999999)]
        public decimal Amount { get; set; }

        // Income or Expense
        [Required]
        [MaxLength(10)]
        public string Type { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime TransactionDate { get; set; }
            = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // Navigation properties
        public User? User { get; set; }

        public Category? Category { get; set; }
    }
}
