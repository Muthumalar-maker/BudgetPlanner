
using System.Security.Claims;
using BudgetPlanner.Api.DTOs.Transactions;
using BudgetPlanner.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetPlanner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionsController(ITransactionService service)
        {
            _service = service;
        }

        private int GetUserId()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out int id))
            {
                throw new UnauthorizedAccessException(
                    "Invalid user token.");
            }

            return id;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();

            var transactions = await _service.GetAllAsync(userId);

            return Ok(transactions);
        }

        // GET: api/transactions/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetUserId();

            var transaction = await _service.GetByIdAsync(id, userId);

            if (transaction == null)
                return NotFound(new { message = "Transaction not found." });

            return Ok(transaction);
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateTransactionDto dto)
        {
            var userId = GetUserId();

            var transaction = await _service.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = transaction.TransactionId },
                transaction);
        }

        // PUT: api/transactions/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, UpdateTransactionDto dto)
        {
            var userId = GetUserId();

            var transaction = await _service.UpdateAsync(
                id, dto, userId);

            if (transaction == null)
                return NotFound(new { message = "Transaction not found." });

            return Ok(transaction);
        }

        // DELETE: api/transactions/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            var deleted = await _service.DeleteAsync(id, userId);

            if (!deleted)
                return NotFound(new { message = "Transaction not found." });

            return Ok(new { message = "Transaction deleted successfully." });
        }
    }
}
