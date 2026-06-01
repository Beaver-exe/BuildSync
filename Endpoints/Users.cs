using BuildSync.Data;
using BuildSync.Models;
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
        });

        group.MapPost("/", async (AppDbContext db, User user) =>
        {
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return Results.Ok(user);
        });
    }
}