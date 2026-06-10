using System.Security.Claims;

namespace BuildSync.Services;

public interface ICurrentUserService
{
    int UserId { get; }
    string Email { get; }
}

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public int UserId =>
        int.Parse(
            _httpContextAccessor.HttpContext!
                .User
                .FindFirst(ClaimTypes.NameIdentifier)!
                .Value);

    public string Email =>
        _httpContextAccessor.HttpContext!
            .User
            .FindFirst(ClaimTypes.Email)?
            .Value ?? string.Empty;
}