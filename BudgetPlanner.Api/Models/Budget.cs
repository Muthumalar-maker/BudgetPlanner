
using System.ComponentModel.DataAnnotations;

namespace BudgetPlanner.Api.Models
{
    public class Budget
    {
        public int BudgetId { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        [Range(0.01, 999999999)]
        public decimal MonthlyLimit { get; set; }

        [Range(1, 12)]
        public int Month { get; set; }

        [Range(2000, 2100)]
        public int Year { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        // Navigation properties
        public User? User { get; set; }

        public Category? Category { get; set; }
    }
}
