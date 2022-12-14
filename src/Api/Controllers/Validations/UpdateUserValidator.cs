using Api.Controllers.DTOs;
using FluentValidation;

namespace Api.Controllers.Validations;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        Transform(x => x.ProfileName, v => v?.Trim())
            .NotEmpty()
            .Length(3, 30)
            .Matches("[^A-Za-z0-9_.]");
        Transform(x => x.DisplayName, v => v?.Trim())
            .NotEmpty()
            .MaximumLength(50);
    }
}