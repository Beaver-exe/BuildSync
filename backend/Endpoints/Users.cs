using BuildSync.Services;

namespace BuildSync.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users")
        .WithTags("Users")
        .RequireAuthorization();

        group.MapGet("/me", async (UserService userSvc) =>
        {
            var user = await userSvc.GetMyInformationAsync();

            if (user == null)
            {
                return Results.BadRequest("Failed to retrieve user information");
            }

            return Results.Ok(user);
        });

        group.MapGet("/{userId}", async (UserService userSvc, Guid userId) =>
        {
            var user = await userSvc.GetUserDtoAsync(userId);

            if (user == null)
            {
                return Results.BadRequest("Failed to retrieve user information");
            }

            return Results.Ok(user);

        });
    }
}