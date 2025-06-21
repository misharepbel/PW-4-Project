﻿using Microsoft.IdentityModel.Tokens;
using UserService.Infrastructure.Extensions;
using UserService.Infrastructure.Persistence;
using UserService.Application.Extensions;
using Microsoft.AspNetCore.Identity;
using UserService.Domain.Entities;
using UserService.Infrastructure.Seed;
using System.Security.Cryptography;
using Microsoft.OpenApi.Models;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Exceptions;

namespace UserService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddApplication();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "UserService",
                    Version = "v1"
                });

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
            builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
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
                options.Map<UserNotFoundException>(ex => new ProblemDetails
                {
                    Title = ex.Message,
                    Status = StatusCodes.Status404NotFound
                });

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
                        var logger = loggerFactory.CreateLogger("UserService");
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

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
                await context.Database.MigrateAsync();

                await UserSeeder.SeedAsync(context, hasher);
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            await app.RunAsync();
        }
    }
}
