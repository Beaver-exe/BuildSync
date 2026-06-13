using BuildSync.DTOs.Project;
using BuildSync.Services;

namespace BuildSync.EndpoGuids;

public static class Projects
{
    public static void MapProjectEndpoGuids(this WebApplication app)
    {
        var group = app.MapGroup("/projects").RequireAuthorization();
        
        group.MapGet("/", async (ProjectService proj) =>
        {
            GetProjectsResponse projects = await proj.RetrieveProjectsAsync();
            return Results.Ok(projects);
        });

        group.MapGet("/{projectId}", async (ProjectService proj, Guid projectId) =>
        {
            var project = await proj.RetrieveProjectAsync(projectId);

            if (project == null)
            {
                return Results.BadRequest("Error retrieving project details");
            }

            return Results.Ok(new ProjectDtoResponse
            {
                Message = "Project details found",
                Project = project
            });
        });

        group.MapPost("/", async (ProjectService proj, CreateProjectRequest request) =>
        {
            var newProject = await proj.CreateProjectAsync(request);
            return Results.Ok(new CreateProjectResponse
            {
                Project = newProject
            });

        });

        group.MapPatch("/{projectId}", async (ProjectService proj, CreateProjectRequest request, Guid projectId) => 
        {
            var updatedProject = await proj.EditProjectAsync(request, projectId);

            if (updatedProject == null)
            {
                return Results.BadRequest("Error retrieving project details");
            }

            return Results.Ok(new CreateProjectResponse
            {
                Project = updatedProject
            });
        });

        group.MapDelete("/{projectId}", async (ProjectService proj, Guid projectId) =>
        {
           var sucess = await proj.DeleteProjectAsync(projectId) ;

            if (!sucess)
            {
                return Results.BadRequest("Failed to delete project");
            }

            return Results.Ok();
        });
    }
}