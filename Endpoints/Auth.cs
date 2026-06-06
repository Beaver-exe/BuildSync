using BuildSync.Data;
using BuildSync.Models;
using BuildSync.DTOs.Auth;
using BuildSync.Service;

using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BuildSync.Endpoints;

public static class Auth
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        app.MapPost("/register", async (AppDbContext db, RegisterRequest request, TokenService tokenService) =>
        {
            var exists = await db.Users.AnyAsync(u => u.Email == request.Email);

            if (exists)
            {
                return Results.BadRequest("Email Already Exists");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Profession = request.Profession,
                Email = request.Email,
                Password = hashedPassword
            };
            
            db.Users.Add(user);
            await db.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            var accessToken = tokenService.GenerateAccessToken(claims);

            return Results.Ok(new
            {
                Message = "User successfully registered",
                AccessToken = accessToken
            });
        });

        app.MapPost("/login", async (AppDbContext db, LoginRequest request, TokenService tokenService) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (user == null)
            {
                return Results.BadRequest("Incorrect Email or Password");
            }

            bool isMatched = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

            if (!isMatched)
            {
                return Results.BadRequest("Incorrect Email or Password");
            }
            
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            var accessToken = tokenService.GenerateAccessToken(claims);

            return Results.Ok(new
            {
               Message = "Login Successfull",
               AccessToken = accessToken
            });
        });
    }
}