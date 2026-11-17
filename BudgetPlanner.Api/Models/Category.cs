
using System.ComponentModel.DataAnnotations;

namespace BudgetPlanner.Api.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        // Income or Expense
        [Required]
        [MaxLength(10)]
        public string Type { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User? User { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
            = new List<Transaction>();

        public ICollection<Budget> Budgets { get; set; }
            = new List<Budget>();
    }
}
