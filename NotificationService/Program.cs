using NotificationService.Services;
using NotificationService.Messaging;
using Microsoft.OpenApi.Models;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "NotificationService", Version = "v1" });
});

builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
builder.Services.AddHostedService<UserRegisteredConsumer>();
builder.Services.AddHostedService<PasswordResetConsumer>();
builder.Services.AddHostedService<PaymentReceiptConsumer>();

builder.Services.AddProblemDetails(options =>
{
    options.Map<ArgumentException>(ex => new ProblemDetails
    {
        Title = ex.Message,
        Status = StatusCodes.Status400BadRequest
    });

    options.Map<InvalidOperationException>(ex => new ProblemDetails
    {
        Title = ex.Message,
        Status = StatusCodes.Status400BadRequest
    });

    options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);

    options.OnBeforeWriteDetails = (ctx, details) =>
    {
        if (ctx.Response.StatusCode == StatusCodes.Status500InternalServerError)
        {
            var loggerFactory = ctx.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("NotificationService");
            var error = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (error != null)
            {
                logger.LogError(error, "Unhandled exception occurred");
            }
        }
    };
});

var app = builder.Build();

app.UseProblemDetails();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

await app.RunAsync();
