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

    private class ProjectUserResolution
    {
        public Project Project { get; set; } = null!;
        public User User { get; set; } = null!;
    }

    private async Task<ProjectUserResolution?> ResolveAdminUserAsync(int projectId, string email)
    {
        var project = await _auth.GetProjectIfAdminAsync(projectId);

        if (project == null)
        {
            return null;
        }

        var normalizedEmail = email.Trim().ToLower();

        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

        if (user == null)
        {
            return null;
        }

        return new ProjectUserResolution
        {
            Project = project,
            User = user
        };
    }

    public async Task<bool> AddMemberAsync(int projectId, AddMemberRequest request)
    {
        var result = await ResolveAdminUserAsync(projectId, request.Email);

        if (result == null)
        {
            return false;
        }

        var exists = await _db.ProjectUsers.AnyAsync(pu =>
            pu.ProjectId == projectId &&
            pu.UserId == result.User.UserId);

        if (exists)
        {
            return false;
        }

        var projUser = new ProjectUser
        {
            ProjectId = projectId,
            UserId = result.User.UserId,
            Role = request.Role,
            AddedAt = DateTime.UtcNow
        };

        _db.ProjectUsers.Add(projUser);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EditMemberStatus(int projectId, EditMemberRequest request)
    {
        var result = await ResolveAdminUserAsync(projectId, request.Email);

        if (result == null)
        {
            return false;
        }

        var membership = await _db.ProjectUsers
            .FirstOrDefaultAsync(pu =>
                pu.ProjectId == projectId &&
                pu.UserId == result.User.UserId);

        if (membership == null)
        {
            return false;
        }

        membership.Role = request.Role;
        await _db.SaveChangesAsync();

        return true;
    }
}