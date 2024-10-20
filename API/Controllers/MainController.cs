using Application.Notification;


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace API.Controllers;

[ApiController]
public class MainController : ControllerBase
{
    readonly INotificationError _notificationError;

    protected MainController(INotificationError notificationError)
    {
        _notificationError = notificationError;
    }

    protected bool ValidOperation()
        => !_notificationError.HasNotifications();

    protected ActionResult CustomResponse(object? result = null, int statusCode = 200)
    {
        if (ValidOperation())
        {
            return StatusCode(statusCode, new
            {
                success = true,
                data = result
            });
        }

        return BadRequest(new
        {
            success = false,
            errors = _notificationError.GetNotifications().Select(n => n.Message)
        });
    }

    protected ActionResult CustomResponse(ModelStateDictionary modelState)
    {
        if (!modelState.IsValid) NotifyErrorModelInvalid(modelState);
        return CustomResponse();
    }

    protected void NotifyErrorModelInvalid(ModelStateDictionary modelState)
    {
        var erros = modelState.Values.SelectMany(e => e.Errors);
        foreach (var error in erros)
        {
            var errorMsg = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
            NotifyError(errorMsg);
        }
    }

    protected void NotifyError(string message)
        => _notificationError.Handle(new NotificationErrorMessage(message));
}
