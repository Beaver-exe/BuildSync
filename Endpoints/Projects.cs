using BuildSync.Data;
using BuildSync.DTOs.Project;

namespace BuildSync.Endpoints;

public static class Projects
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/project");

        app.MapPost("/create", async (AppDbContext db, CreateProjectRequest request) =>
        {
            
        }).RequireAuthorization();
    }
}