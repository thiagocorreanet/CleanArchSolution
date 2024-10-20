using Application.Commands.User.Create;

using FluentValidation;

namespace Application.Validators.User;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommandRequest>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("The email is required.")
           .NotNull().WithMessage("The email field cannot be null.")
           .EmailAddress().WithMessage("The provided email is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Field is empty, the password is required.")
            .NotNull().WithMessage("The password is null, please enter a valid password.")
            .MinimumLength(8).WithMessage("The password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("The password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("The password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("The password must contain at least one number.")
            .Matches("[^a-zA-Z0-9]").WithMessage("The password must contain at least one special character.");
    }
}
