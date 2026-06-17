using BuildSync.DTOs.Auth;
using BuildSync.Services;

namespace BuildSync.Endpoints;

public static class Auth
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth")
        .WithTags("Authorisation");

        group.MapPost("/register", async (AuthService auth, RegisterRequest request) =>
        {
            var token = await auth.RegisterAsync(request);

            if (token == null)
            {
                return Results.BadRequest("Email already registered, try login instead");
            }

            return Results.Ok(new AuthResponse
            {
                Message = "User successfully registered",
                AccessToken = token
            });

        });

        group.MapPost("/login", async (AuthService auth, LoginRequest request) =>
        {
            var token = await auth.LoginAsync(request);

            if (token == null)
            {
                return Results.BadRequest("Incorrect Email or Password");
            }

            return Results.Ok(new AuthResponse
            {
               Message = "Login Successfull",
               AccessToken = token
            });
        });
    }
}