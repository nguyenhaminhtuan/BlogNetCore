using Api.Controllers.DTOs;
using FluentValidation;

namespace Api.Controllers.Validations;

public class UpdateArticleValidator : AbstractValidator<UpdateArticleDto>
{
    public UpdateArticleValidator()
    {
        Transform(x => x.Title, v => v?.Trim())
            .NotEmpty()
            .MaximumLength(200);
        RuleFor(x => x.Content)
            .NotEmpty();
    }
}