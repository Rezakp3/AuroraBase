using Api.Extentions;
using Application;
using Application.Common.Models;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Utils.CustomAttributes;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var appSetting = builder.Configuration.Get<AppSetting>() ?? new();
builder.Services.AddSingleton(appSetting);

// Add Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();

// Controllers & Validation
builder.Services.AddControllers(opts => 
    opts.Filters.Add<ValidateModelStateAttribute>()); 

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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

var app = builder.Build();

// ✅ Seed کردن داده‌ها در Startup
if (app.Environment.IsDevelopment())
{
    await Infrastructure.DependencyInjection.SeedDatabaseAsync(app.Services);
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
app.MapControllers();

app.Run();
