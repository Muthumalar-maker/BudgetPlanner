
using System.ComponentModel.DataAnnotations;

namespace BudgetPlanner.Api.DTOs.Categories;

public class UpdateCategoryDto
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string CategoryName { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Income|Expense)$",
        ErrorMessage = "Type must be Income or Expense.")]
    public string Type { get; set; } = string.Empty;
}
