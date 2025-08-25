using FastEndpoints;
using SkillsPortal.API.Features.Projects;
using SkillsPortal.Core;
using SkillsPortal.Core.Infrastructure.DbContext;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<SkillsContext>();
builder.Services.AddScoped<IRelationalValidationService, RelationalValidationService>();
builder.Services.AddFastEndpoints();

builder.Services.AddSingleton<IProjectRepository, ProjectRepository>();
builder.Services.AddSingleton<IProjectService, ProjectService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseDefaultExceptionHandler()
    .UseFastEndpoints();

app.UseHttpsRedirection();