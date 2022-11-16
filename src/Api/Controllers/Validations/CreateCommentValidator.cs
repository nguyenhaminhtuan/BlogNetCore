using Api.Controllers.DTOs;
using FluentValidation;

namespace Api.Controllers.Validations;

public class CreateCommentValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty();
    }
}