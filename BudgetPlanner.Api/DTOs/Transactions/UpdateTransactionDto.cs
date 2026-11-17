
using System.ComponentModel.DataAnnotations;

namespace BudgetPlanner.Api.DTOs.Transactions
{
    public class UpdateTransactionDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Required]
        [Range(0.01, 999999999)]
        public decimal Amount { get; set; }

        [Required]
        [RegularExpression("^(Income|Expense)$",
            ErrorMessage = "Type must be Income or Expense.")]
        public string Type { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime? TransactionDate { get; set; }
    }
}
