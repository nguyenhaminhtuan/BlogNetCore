using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class ErrorsController : ApiControllerBase
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error")]
    public IActionResult HandleError()
    {
        return Problem(
            title: "Something went wrong.",
            detail: "An error occured while processing your request.",
            statusCode: StatusCodes.Status500InternalServerError);
    }
}