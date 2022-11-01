using Api.Controllers.DTOs;
using FluentValidation;

namespace Api.Controllers.Validations;

public class CreateArticleValidator : AbstractValidator<CreateArticleDto>
{
    public CreateArticleValidator()
    {
        Transform(x => x.Title, v => v?.Trim())
            .NotEmpty()
            .MaximumLength(150);
        RuleFor(x => x.Content)
            .NotEmpty();
    }
}