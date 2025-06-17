using NotificationService.Services;
using NotificationService.Messaging;
using Microsoft.OpenApi.Models;

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

await app.RunAsync();
