using Microsoft.AspNetCore.Mvc;

namespace BlogNetCore.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult NotFoundOrResult(object? o, IActionResult result)
    {
        return o is null ? NotFound() : result;
    }
    
    protected async Task<IActionResult> NotFoundOrResult(object? o, Func<Task<IActionResult>> func)
    {
        return o is null ? NotFound() : await func();
    }
}