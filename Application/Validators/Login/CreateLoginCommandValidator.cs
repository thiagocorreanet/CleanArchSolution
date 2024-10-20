using Application.Commands.Login.Create;

using FluentValidation;

namespace Application.Validators.Login;

public class CreateLoginCommandValidator : AbstractValidator<CreateLoginCommandRequest>
{
    public CreateLoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Field is empty, the email is required.")
            .NotNull().WithMessage("The email is null, please enter a valid email.");

        RuleFor(x => x.Passwork)
            .NotEmpty().WithMessage("Field is empty, the password is required.")
            .NotNull().WithMessage("The email is null, please enter a valid email.");
    }
}
