using Microsoft.AspNetCore.Mvc;
using SmartBudget.API.Common;
using SmartBudget.API.Services;

namespace SmartBudget.API.Controllers;

[Route("api/v1/cycles")]
public class CyclesController : BaseApiController
{
    private readonly ICycleService _cycles;
    public CyclesController(ICycleService cycles) => _cycles = cycles;

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var result = await _cycles.GetActiveCycleAsync(UserId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}