using BuildSync.Data;
using BuildSync.DTOs.Users;
using System. Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace BuildSync.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users");

        group.MapGet("/", async (AppDbContext db) =>
        {
           return await db.Users.ToListAsync();
        }).RequireAuthorization();

        group.MapGet("/{userId}", async (AppDbContext db, int userId, ClaimsPrincipal userPrincipal) =>
        {
            var currentUserId = int.Parse(
                userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"
            );

            if (currentUserId != userId)
            {
                return Results.Forbid();
            }

            var user = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            
            if (user == null)
            {
                return Results.NotFound("User with that Id does not exist");
            }

            var dto = new UserDto
            {   
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Profession = user.Profession,
                Email = user.Email
            };

            return Results.Ok(new
            {
                Message = "User with matching id found",
                dto
            });

        }).RequireAuthorization();

    }
}