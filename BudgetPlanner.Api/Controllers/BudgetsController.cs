
using BudgetPlanner.Api.DTOs.Budget;
using BudgetPlanner.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BudgetPlanner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BudgetsController : ControllerBase
    {
        private readonly IBudgetService _budgetService;

        public BudgetsController(IBudgetService budgetService)
        {
            _budgetService = budgetService;
        }

        // Get logged-in user's ID from JWT
        private int GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.TryParse(userId, out int id) ? id : 0;
        }

        // GET: api/budgets
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();

            if (userId <= 0)
                return Unauthorized();

            var budgets = await _budgetService.GetAllAsync(userId);

            return Ok(budgets);
        }

        // GET: api/budgets/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();

            if (userId <= 0)
                return Unauthorized();

            var budget = await _budgetService.GetByIdAsync(id, userId);

            if (budget == null)
                return NotFound(new { message = "Budget not found." });

            return Ok(budget);
        }

        // GET: api/budgets/month/11/year/2026
        [HttpGet("month/{month:int}/year/{year:int}")]
        public async Task<IActionResult> GetByMonthAndYear(
            int month,
            int year)
        {
            var userId = GetUserId();

            if (userId <= 0)
                return Unauthorized();

            try
            {
                var budgets = await _budgetService.GetByMonthAndYearAsync(
                    userId, month, year);

                return Ok(budgets);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/budgets
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateBudgetDto dto)
        {
            var userId = GetUserId();

            if (userId <= 0)
                return Unauthorized();

            try
            {
                var budget = await _budgetService.CreateAsync(userId, dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = budget.BudgetId },
                    budget);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/budgets/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateBudgetDto dto)
        {
            var userId = GetUserId();

            if (userId <= 0)
                return Unauthorized();

            try
            {
                var budget = await _budgetService.UpdateAsync(
                    id, userId, dto);

                if (budget == null)
                    return NotFound(new { message = "Budget not found." });

                return Ok(budget);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // DELETE: api/budgets/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            if (userId <= 0)
                return Unauthorized();

            var deleted = await _budgetService.DeleteAsync(id, userId);

            if (!deleted)
                return NotFound(new { message = "Budget not found." });

            return Ok(new { message = "Budget deleted successfully." });
        }
    }
}
