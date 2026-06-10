using BuildSync.DTOs.Project;
using BuildSync.Services;

namespace BuildSync.Endpoints;

public static class Projects
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/project").RequireAuthorization();
        
        app.MapGet("/", async (ProjectService project) =>
        {
            GetProjectResponse projects = await project.RetrieveProjectsAsync();
            return Results.Ok(projects);
        });

        app.MapPost("/", async (ProjectService project, CreateProjectRequest request) =>
        {
            var newProject = await project.CreateProjectAsync(request);
            return Results.Ok(new CreateProjectResponse
            {
                Project = newProject
            });

        });
    }
}