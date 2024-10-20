using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace API.Extension;

public static class ServiceExtension
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            // Configuração básica do Swagger
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "WebAPI",
                Version = "v1",
                Contact = new OpenApiContact
                {
                    Name = "Add the project name.",
                    Url = new Uri("https://www.meusite.com.br/")
                },
                Description = @"Welcome to our Swagger documentation! We are happy to have you here. In this documentation, you will find a complete and detailed description of all the endpoints available in our API."
            });

            // Inclui comentários XML para documentação
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            // Configuração da autenticação JWT no Swagger
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insert the JWT token like this: Bearer {your token here}."
            };

            c.AddSecurityDefinition("Bearer", securityScheme);

            var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                securityScheme,
                new List<string>()
            }
        };

            c.AddSecurityRequirement(securityRequirement);
        });
    }

    public static void UseSwaggerUI(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.DocumentTitle = "Add your title.";
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");

            c.DefaultModelExpandDepth(2);
            c.DefaultModelRendering(ModelRendering.Model);
            c.DefaultModelsExpandDepth(-1);
            c.DisplayOperationId();
            c.DisplayRequestDuration();
            c.EnableDeepLinking();
            c.EnableFilter();
            c.ShowExtensions();
            c.EnableValidator();
            c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete);
            c.EnableDeepLinking();
        });
    }
}
