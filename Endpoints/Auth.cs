using BuildSync.Data;
using BuildSync.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildSync.Endpoints;

public static class Auth
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

    }
}