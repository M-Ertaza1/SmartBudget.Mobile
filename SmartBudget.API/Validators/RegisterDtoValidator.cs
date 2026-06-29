using FluentValidation;
using SmartBudget.API.DTOs.Auth;

namespace SmartBudget.API.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Mobile).NotEmpty()
            .Must(m => m.Count(char.IsDigit) >= 10)
            .WithMessage("Please enter a valid mobile number.");
        RuleFor(x => x.SalaryDay).InclusiveBetween(1, 31);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.");
        RuleFor(x => x.ConfirmPassword).Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
    }
}