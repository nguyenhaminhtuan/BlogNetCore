using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    [NonAction]
    protected IActionResult BadRequest(string? title, string? detail)
    {
        return ValidationProblem(detail, title: title);
    }

    [NonAction]
    protected IActionResult BadRequest(string detail)
    {
        return ValidationProblem(detail);
    }
}