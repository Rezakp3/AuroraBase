using Api.Attributes;
using Api.Extentions;
using Application;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Aurora.Cache;
using Aurora.Captcha;
using Aurora.Jwt;
using Aurora.Jwt.Models;
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
    //.WriteTo.Seq("http://localhost:5341", apiKey: "NPtEAuuE6SpkH0u0kMw3")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();


// Configuration
var services = builder.Services;
var appsetting = builder.Configuration.Get<AppSetting>() ?? new();

services.AddSingleton(appsetting)
    .AddSingleton(appsetting.AuroraLog);

builder.Services.AddProblemDetails(x => ApiResult.Fail(null, 500));
services.AddCache();
services.AddLogger();
services.AddCaptcha();
services.AddJwt();
// Add Layers
services.AddApplication();
services.AddInfrastructure(builder.Configuration);

//دریافت اطلاعات jwtsetting از دیتابیس
IServiceProvider serviceProvider = services.BuildServiceProvider();
var setting = serviceProvider.GetRequiredService<ISettingRepository>();
var jwtSetting = await setting.GetByGroupAsync<JwtSettings>("JwtSettings");
services.AddSingleton(jwtSetting);

services.AddExceptionHandler<ExceptionHandler>();
services.AddHttpContextAccessor();

// CORS - Development: Allow Everything
services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.SecretKey)),

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
await app.Services.SeedDatabaseAsync();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseExceptionHandler(new ExceptionHandlerOptions
{
    // همیشه لاگ بزن (هیچوقت Suppress نکن)
    SuppressDiagnosticsCallback = context => false
});

app.UseCors("DevCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.UseJwtBlocklist();
app.MapControllers();

app.Run();
