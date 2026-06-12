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

    public async Task<bool> AddMemberAsync(int projectId, MemberRequest request)
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

    public async Task<bool> EditMemberStatus(int projectId, MemberRequest request)
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

    public async Task<bool> RemoveMemberStatusAsync(int projectId, RemoveMemberRequest request)
    {
        var requesterId = _currentUser.UserId;

        var resolved = await ResolveAdminUserAsync(projectId, request.Email);

        if (resolved == null) {
            return false;
        }

        var project = resolved.Project;

        var requester = await _db.ProjectUsers
            .FirstOrDefaultAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == requesterId);

        if (requester == null) {
            return false;
        }

        var target = await _db.ProjectUsers
            .FirstOrDefaultAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == resolved.User.UserId);

        if (target == null) {
            return false;
        }

        if (requester.UserId == target.UserId) {
            return false;
        }

        var isOwner = project.ProjectOwnerId == requesterId;
        var canRemove =
            (isOwner && true) ||
            (requester.Role == "Admin" && target.Role == "Member");

        if (!canRemove) {
            return false;
        }
        
        _db.ProjectUsers.Remove(target);
        await _db.SaveChangesAsync();

        return true;
    }
}