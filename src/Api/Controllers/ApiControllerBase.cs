using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json" , "application/problem+json")]
[Consumes("application/json")]
[SwaggerResponse(
    statusCode: StatusCodes.Status500InternalServerError,
    description: "Can't handle request because got an internal server error",
    typeof(ProblemDetails))]
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

    protected IActionResult Unauthorized(string reason)
    {
        return Problem(
            title: "Unauthorized",
            detail: reason,
            statusCode: (int)HttpStatusCode.Unauthorized);
    }

    [NonAction]
    protected IActionResult Forbid(string? reason)
    {
        return Problem(
            title: "Forbidden",
            detail: reason ?? "Permission denied",
            statusCode: (int)HttpStatusCode.Forbidden);
    }

    protected IActionResult NotFound(string reason)
    {
        return Problem(
            title: "Not found",
            detail: reason,
            statusCode: (int)HttpStatusCode.NotFound);
    }
}