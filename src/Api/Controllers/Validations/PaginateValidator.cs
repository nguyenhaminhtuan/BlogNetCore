using Api.Models;
using FluentValidation;

namespace Api.Controllers.Validations;

public class PaginateValidator : AbstractValidator<PaginateQuery>
{
    public PaginateValidator()
    {
        RuleFor(x => x.PageIndex)
            .GreaterThan(0);
        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThan(100);
    }
}