using Application.Commands.Login.Create;
using Application.Commands.User.Create;
using Application.Notification;

using FluentValidation;
using FluentValidation.AspNetCore;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

namespace Application;

public static class DependencyInjectionApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddMediatR(typeof(CreateUserCommandHendler))
            .AddNotifications()
            .AddValidation();

        return services;
    }

    private static IServiceCollection AddNotifications(this IServiceCollection services)
    {
        services.AddScoped<INotificationError, NotificationError>();

        return services;
    }

    private static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services
            .AddFluentValidationAutoValidation()
            .AddValidatorsFromAssemblyContaining<CreateLoginCommandRequest>();

        return services;
    }

    public static void AddSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        Serilog.Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .MinimumLevel.Warning()
            .Enrich.WithEnvironmentName()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .WriteTo.Console()
            .WriteTo.MSSqlServer(
                connectionString: configuration.GetConnectionString("DefaultConnection"),
                sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
                {
                    TableName = "Serilogs",
                    AutoCreateSqlTable = true
                })
            .CreateLogger();

    }
}
