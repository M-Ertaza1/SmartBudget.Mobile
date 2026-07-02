using Microsoft.AspNetCore.Mvc;
using SmartBudget.API.DTOs.Budget;
using SmartBudget.API.Services;

namespace SmartBudget.API.Controllers;

[Route("api/v1/expenses")]
public class ExpensesController : BaseApiController
{
    private readonly IExpenseService _expenses;
    public ExpensesController(IExpenseService expenses) => _expenses = expenses;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _expenses.GetExpensesAsync(UserId);
        return Ok(result);
    }

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _expenses.GetCategoriesAsync(UserId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateExpenseDto dto)
    {
        var result = await _expenses.CreateExpenseAsync(UserId, dto);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}