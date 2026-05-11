using Asp.Versioning.ApiExplorer;
using EnterpriseCommerce.API.Middlewares;
using EnterpriseCommerce.Application.Behaviors;
using EnterpriseCommerce.Application.Features.Products.Commands;
using EnterpriseCommerce.Application.Features.Products.Validators;
using EnterpriseCommerce.Application.Interfaces;
using EnterpriseCommerce.Domain.Interfaces;
using EnterpriseCommerce.Infrastructure.BackgroundServices;
using EnterpriseCommerce.Infrastructure.Connections;
using EnterpriseCommerce.Infrastructure.Messaging;
using EnterpriseCommerce.Infrastructure.Repositories;
using EnterpriseCommerce.Infrastructure.Services;
using FluentValidation;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using EnterpriseCommerce.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

//builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SqlConnectionFactory>();
builder.Services.AddHostedService<
    UserRegistrationConsumer>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IJwtTokenGenerator,
    JwtTokenGenerator>();
builder.Services.AddScoped<
    IRefreshTokenRepository,
    RefreshTokenRepository>();
var jwtSettings =
    builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings["Issuer"],

                ValidAudience = jwtSettings["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings["Secret"]!))
            };
    });
builder.Services.AddScoped<
    ICacheService,
    RedisCacheService>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateProductCommand>();
});
// Hangfire settting 
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(
        builder.Configuration
            .GetConnectionString("DefaultConnection"));
});

builder.Services.AddHangfireServer();
builder.Services.AddScoped<EmailJobService>();
builder.Services.AddResponseCompression();
//Hangfire end
builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(LoggingBehavior<,>));
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion =
        new Asp.Versioning.ApiVersion(1, 0);

    options.AssumeDefaultVersionWhenUnspecified = true;

    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
}); ;
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        "fixed",
        limiterOptions =>
        {
            limiterOptions.PermitLimit = 5;

            limiterOptions.Window =
                TimeSpan.FromSeconds(10);
        });
});

builder.Services.AddTransient(
    typeof(IPipelineBehavior<,>),
    typeof(ValidationBehavior<,>));
//RabbitMQ changes start
builder.Services.AddSingleton<
    IMessageBroker,
    RabbitMqProducer>();

//RabbitMQ changes end

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];
});
// Redis changes End

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

///app.UseSwagger();

//app.UseSwaggerUI();

app.Use(async (context, next) =>
{
    context.Response.Headers
        .Append("X-Frame-Options", "DENY");

    context.Response.Headers
        .Append("X-Content-Type-Options", "nosniff");

    context.Response.Headers
        .Append("Referrer-Policy",
            "strict-origin-when-cross-origin");

    await next();
});

app.UseHttpsRedirection();
app.MapOpenApi();
app.UseRateLimiter();
app.UseHangfireDashboard();
app.MapScalarApiReference();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<AuditMiddleware>();
app.MapControllers();
app.UseResponseCompression();
app.Run();