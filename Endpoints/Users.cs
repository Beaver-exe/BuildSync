using BuildSync.Services;

namespace BuildSync.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users").RequireAuthorization();

        group.MapGet("/me", async (UserService userSvc) =>
        {
            var user = await userSvc.GetMyInformationAsync();

            if (user == null)
            {
                return Results.BadRequest("Failed to retrieve user information");
            }

            return Results.Ok(user);
        });

        group.MapGet("/{userGuid}", async (UserService userSvc, Guid userGuid) =>
        {
            var user = await userSvc.GetUserDtoAsync(userGuid);

            if (user == null)
            {
                return Results.BadRequest("Failed to retrieve user information");
            }

            return Results.Ok(user);

        });
    }
}