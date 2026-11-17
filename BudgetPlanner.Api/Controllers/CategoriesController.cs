
using System.Security.Claims;
using BudgetPlanner.Api.DTOs.Categories;
using BudgetPlanner.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetPlanner.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    private int GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException(
                "Invalid user authentication.");
        }

        return parsedUserId;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetUserId();

        var categories = await _categoryService.GetAllAsync(userId);

        return Ok(categories);
    }

    // GET: api/categories/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetUserId();

        var category = await _categoryService.GetByIdAsync(id, userId);

        if (category == null)
            return NotFound(new { message = "Category not found." });

        return Ok(category);
    }

    // POST: api/categories
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryDto dto)
    {
        var userId = GetUserId();

        var category = await _categoryService.CreateAsync(dto, userId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = category.CategoryId },
            category);
    }

    // PUT: api/categories/1
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateCategoryDto dto)
    {
        var userId = GetUserId();

        var category = await _categoryService.UpdateAsync(
            id, dto, userId);

        if (category == null)
            return NotFound(new { message = "Category not found." });

        return Ok(category);
    }

    // DELETE: api/categories/1
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserId();

        var deleted = await _categoryService.DeleteAsync(id, userId);

        if (!deleted)
            return NotFound(new { message = "Category not found." });

        return Ok(new { message = "Category deleted successfully." });
    }
}
