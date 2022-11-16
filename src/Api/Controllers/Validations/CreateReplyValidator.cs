using Api.Controllers.DTOs;
using FluentValidation;

namespace Api.Controllers.Validations;

public class CreateReplyValidator : AbstractValidator<CreateReplyDto>
{
    public CreateReplyValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty();
    }
}