using CartService.Application.Extensions;
using CartService.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System;

namespace CartService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "CartService", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer eyJhbGci...\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var publicKeyEnv = Environment.GetEnvironmentVariable("JWT_PUBLIC_KEY");
                    if (string.IsNullOrWhiteSpace(publicKeyEnv))
                        throw new InvalidOperationException("JWT_PUBLIC_KEY is not set.");
                    publicKeyEnv = publicKeyEnv.Replace("\\n", "\n");
                    var rsa = RSA.Create();
                    rsa.ImportFromPem(publicKeyEnv.ToCharArray());

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = "UserService",
                        ValidAudience = "TeaShop",
                        IssuerSigningKey = new RsaSecurityKey(rsa)
                    };
                });

            builder.Services.AddAuthorization();

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
                        var logger = loggerFactory.CreateLogger("CartService");
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

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
