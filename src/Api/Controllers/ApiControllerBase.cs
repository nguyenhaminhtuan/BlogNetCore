using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json" , "application/problem+json")]
[Consumes("application/json")]
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

    public override BadRequestObjectResult BadRequest(ModelStateDictionary modelState)
    {
        if (modelState is null)
            throw new ArgumentNullException(nameof(modelState));

        var problem = ProblemDetailsFactory.CreateValidationProblemDetails(HttpContext, modelState);
        return new BadRequestObjectResult(problem);
    }
}