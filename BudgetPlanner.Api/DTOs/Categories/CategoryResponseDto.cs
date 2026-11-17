
namespace BudgetPlanner.Api.DTOs.Categories;

public class CategoryResponseDto
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
