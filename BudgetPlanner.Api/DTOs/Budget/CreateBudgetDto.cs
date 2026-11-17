using System.ComponentModel.DataAnnotations;

namespace BudgetPlanner.Api.DTOs.Budget
{
    public class CreateBudgetDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [Range(0.01, 999999999)]
        public decimal MonthlyLimit { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        [Range(2000, 2100)]
        public int Year { get; set; }
    }
}