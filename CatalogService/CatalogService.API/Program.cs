﻿
using CatalogService.Application.Extensions;
using CatalogService.Infrastructure.Extensions;
using CatalogService.Infrastructure.Persistence;
using CatalogService.Infrastructure.Seeders;
using CatalogService.Application.Interfaces;
using AutoMapper;
using CatalogService.Application.DTOs;
using CatalogService.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
namespace CatalogService.API
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CatalogService",
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
                        var logger = loggerFactory.CreateLogger("CatalogService");
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

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
                await context.Database.MigrateAsync();

                await CatalogSeeder.SeedAsync(context);

                var repo = scope.ServiceProvider.GetRequiredService<ICatalogRepository>();
                var producer = scope.ServiceProvider.GetRequiredService<IProductCacheProducer>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var products = await repo.GetAllProductsAsync();
                var dto = products.Select(p => mapper.Map<ProductDto>(p)).ToList();
                await producer.PublishAsync(new ProductCacheEvent { Products = dto });
            }

            // Configure the HTTP request pipeline.

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
