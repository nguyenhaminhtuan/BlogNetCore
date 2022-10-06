using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace BlogNetCore.Controllers;

public class ErrorsController : ControllerBase
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error")]
    public IActionResult HandleError()
    {
        return Problem(
            title: "Something went wrong.",
            detail: "An error occured while processing your request.",
            statusCode: (int)HttpStatusCode.InternalServerError);
    }
}