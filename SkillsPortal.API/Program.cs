using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SkillsPortal.API.Features.Employees;
using SkillsPortal.API.Features.Projects;
using SkillsPortal.API.Features.Projects.Validators;
using SkillsPortal.API.Features.Skills;
using SkillsPortal.API.Middleware;
using SkillsPortal.API.Shared;
using SkillsPortal.Core.Infrastructure.DbContext;

namespace SkillsPortal.API;

public abstract class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// configure built-in logging
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        builder.Logging.AddSimpleConsole(options => options.SingleLine = true);

        builder.Services.AddFastEndpoints();
        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<SkillsContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IRelationalValidationService, RelationalValidationService>();


        builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ISkillsRepository, SkillsRepository>();
        builder.Services.AddScoped<CreateProjectValidator>();
        builder.Services.AddScoped<IProjectService, ProjectService>();

        builder.Services.AddValidatorsFromAssemblyContaining<CreateProjectValidator>();

        var app = builder.Build();

        app.UseMiddleware<RequestLoggingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseDefaultExceptionHandler()
            .UseFastEndpoints(c =>
            {
                c.Endpoints.RoutePrefix = "api";
                c.Errors.GeneralErrorsField = "errors";
            });

        app.UseHttpsRedirection();

        app.Run();
    }
}