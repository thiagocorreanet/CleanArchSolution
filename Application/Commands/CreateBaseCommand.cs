using Application.Notification;

using Core.Entities;

using FluentValidation;
using FluentValidation.Results;

namespace Application.Commands;

public class CreateBaseCommand
{
    readonly INotificationError _notificationError;

    public CreateBaseCommand(INotificationError notificationError)
    {
        _notificationError = notificationError;
    }

    protected void Notify(ValidationResult validationResult)
    {
        foreach (var error in validationResult.Errors)
        {
            Notify(error.ErrorMessage);
        }
    }

    protected void Notify(string message)
    {
        _notificationError.Handle(new NotificationErrorMessage(message));
    }

    protected bool RunValidation<TV, TE>(TV validation, TE entity) where TV : AbstractValidator<TE> where TE : BaseEntity
    {
        var validator = validation.Validate(entity);

        if (validator.IsValid) return true;

        Notify(validator);

        return false;
    }

    protected bool ValidationIdentity<TV, TE>(TV validation, TE entity) where TV : AbstractValidator<TE>
    {
        var validator = validation.Validate(entity);

        if (validator.IsValid) return true;

        Notify(validator);

        return false;
    }
}
