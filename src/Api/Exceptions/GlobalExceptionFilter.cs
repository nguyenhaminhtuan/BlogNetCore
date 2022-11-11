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
        var problem = _problemDetailsFactory.CreateProblemDetails(context.HttpContext);
        switch (context.Exception)
        {
            case ConflictException:
                problem.Status = StatusCodes.Status409Conflict;
                problem.Title = "Conflict";
                problem.Detail = context.Exception.Message;
                context.Result = new ObjectResult(problem)
                {
                    StatusCode = problem.Status
                };
                context.ExceptionHandled = true;
                break;
            case NotImplementedException:
                problem.Status = StatusCodes.Status501NotImplemented;
                problem.Title = "Not implemented";
                problem.Detail = context.Exception.Message;
                context.Result = new ObjectResult(problem)
                {
                    StatusCode = problem.Status
                };
                context.ExceptionHandled = true;
                break;
        }
    }
}