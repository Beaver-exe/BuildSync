using BuildSync.Data;
using BuildSync.Models;
using BuildSync.DTOs.Auth;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BuildSync.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokenService;

    public AuthService(AppDbContext db, TokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<string?> RegisterAsync(RegisterRequest request)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == request.Email);

        if (exists)
        {
            return null;
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
        
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return GenerateToken(user);
    }

    public async Task<string?> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        
        if (user == null)
        {
            return null;
        }

        bool isMatched = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!isMatched)
        {
            return null;
        }
        
        return GenerateToken(user);
    }


    private string GenerateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var accessToken = _tokenService.GenerateAccessToken(claims);

        return accessToken;
    }


}