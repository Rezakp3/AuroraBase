using Core.ViewModel.Base;
using Core;
using Gateway;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Application;
using Core.CustomAttributes;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// get appsetting and create init setting
var appSetting = builder.Configuration.Get<AppSetting>() ?? new();
builder.Services.AddSingleton(appSetting);

builder.Services.AddCore();
builder.Services.AddGateway(appSetting);
builder.Services.AddApplication();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<DBContext>(x =>
    {
        x.UseSqlServer(appSetting.ConnectionStrings.DefaultConnection);
        x.EnableSensitiveDataLogging();
    });

builder.Services.AddControllers(opts => 
    opts.Filters.Add(typeof(ValidateModelStateAttribute)));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    OpenApiSecurityScheme jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Put *ONLY* your JWT Bearer token on textbox below!",

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
    c.DescribeAllParametersInCamelCase();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiCore", Version = "v1.0.1" });

    c.EnableAnnotations();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = [
                            new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" },
                            new() { Url = $"{httpReq.Scheme}s://{httpReq.Host.Value}" },
                        ];
        });
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "Api v1");
    });
}

if (!Path.Exists(Path.Combine(Directory.GetCurrentDirectory(), "assets")))
    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "assets"));

app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "assets")),
    RequestPath = new PathString("/assets"),
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Content-Security-Policy", "default-src 'self';");
    await next();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
