using BuildSync.Data;
using BuildSync.DTOs.Member;
using BuildSync.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildSync.Services;

public class MemberService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ProjectAuthorizationService _auth;

    public MemberService(AppDbContext db, ICurrentUserService currentUser, ProjectAuthorizationService auth)
    {
        _db = db;
        _currentUser = currentUser;
        _auth = auth;
    }

    public async Task<bool> AddMemberAsync(int projectId, AddMemberRequest request)
    {
        var project = await _auth.GetProjectIfAdminAsync(projectId);

        if (project == null)
        {
            return false;
        }

        var email = request.Email.Trim().ToLower();
        var newUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (newUser == null)
        {
            return false;
        }

        var exists = await _db.ProjectUsers.AnyAsync(pu =>
            pu.ProjectId == projectId &&
            pu.UserId == newUser.UserId);

        if (exists)
        {
            return false;
        }

        var projUser = new ProjectUser
        {
            ProjectId = projectId,
            UserId = newUser.UserId,
            Role = request.Role,
            AddedAt = DateTime.UtcNow
        };

        _db.ProjectUsers.Add(projUser);
        await _db.SaveChangesAsync();

        return true;
    }
}