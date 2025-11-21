using Api.Attributes;
using Api.Extentions;
using Application;
using Application.Common.Models;
using Aurora.Cache;
using Aurora.Captcha;
using Aurora.Jwt;
using Aurora.Logger;
using Infrastructure;
using Infrastructure.Security.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
// Configuration
var services = builder.Services;
var appsetting = builder.Configuration.Get<AppSetting>() ?? new();
services.AddSingleton(appsetting)
    .AddSingleton(appsetting.JwtSettings)
    .AddSingleton(appsetting.AuroraLog);

services.AddCache();
services.AddLogger(); 
services.AddCaptcha();
services.AddJwt();

// Add Layers
services.AddApplication();
services.AddInfrastructure(builder.Configuration);

services.AddHttpContextAccessor();

// Controllers & Validation
services.AddControllers(opts =>
    opts.Filters.Add<ValidateModelStateAttribute>());

// Swagger
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put **ONLY** your JWT Bearer token below!",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    
    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuroraBase API", Version = "v1.0" });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appsetting.JwtSettings.SecretKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 2. ثبت Policy
services.AddAuthorizationBuilder()
                    .AddPolicy(AutoPermissionAttribute.PolicyName, static policy =>
                    { policy.Requirements.Add(new DynamicPermissionRequirement()); });


var app = builder.Build();

// ✅ Seed کردن داده‌ها در Startup
if (app.Environment.IsDevelopment())
{
    await app.Services.SeedDatabaseAsync();
}

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseJwtBlocklist();
app.MapControllers();

app.Run();
