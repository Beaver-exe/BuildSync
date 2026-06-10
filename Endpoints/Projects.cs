using BuildSync.DTOs.Project;
using BuildSync.Services;

namespace BuildSync.Endpoints;

public static class Projects
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/project").RequireAuthorization();
        
        app.MapGet("/", async (ProjectService proj) =>
        {
            GetProjectsResponse projects = await proj.RetrieveProjectsAsync();
            return Results.Ok(projects);
        });

        app.MapGet("/{projectId}", async (ProjectService proj, int projectId) =>
        {
            var project = await proj.RetrieveProjectAsync(projectId);

            if (project == null)
            {
                return Results.BadRequest("Error retrieving project details");
            }

            return Results.Ok(project);
        });

        app.MapPost("/", async (ProjectService proj, CreateProjectRequest request) =>
        {
            var newProject = await proj.CreateProjectAsync(request);
            return Results.Ok(new CreateProjectResponse
            {
                Project = newProject
            });

        });
    }
}