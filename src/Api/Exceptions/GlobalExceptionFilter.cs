using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Api.Exceptions;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ProblemDetailsFactory _problemDetailsFactory;

    public GlobalExceptionFilter(ProblemDetailsFactory problemDetailsFactory)
    {
        _problemDetailsFactory = problemDetailsFactory;
    }
    
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is ConflictException)
        {
            var problem = _problemDetailsFactory.CreateProblemDetails(
                context.HttpContext,
                (int)HttpStatusCode.Conflict, 
                title: "Conflict",
                detail: context.Exception.Message);
            context.Result = new ObjectResult(problem)
            {
                StatusCode = (int)HttpStatusCode.Conflict
            };
            context.ExceptionHandled = true;
        }
    }
}