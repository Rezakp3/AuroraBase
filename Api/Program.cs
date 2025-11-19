using Api.Extentions;
using Application;
using Aurora.Jwt.Models;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var services = builder.Services;
services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


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
app.MapControllers();

app.Run();
