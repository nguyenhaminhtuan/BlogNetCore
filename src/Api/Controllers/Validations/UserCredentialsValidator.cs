using Api.Controllers.DTOs;
using FluentValidation;

namespace Api.Controllers.Validations;

public class UserCredentialsValidator  : AbstractValidator<UserCredentialsDto>
{
    public UserCredentialsValidator()
    {
        Transform(x => x.Username, v => v?.Trim())
            .NotEmpty()
            .EmailAddress();
        Transform(x => x.Password, v => v?.Trim())
            .NotEmpty()
            .Length(6, 150);
    }
}