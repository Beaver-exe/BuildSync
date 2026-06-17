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

    private async Task<Project?> ResolveProjectAsync(Guid projectGuid)
    {
        return await _db.Projects
            .FirstOrDefaultAsync(p => p.GProjectId == projectGuid);
    }

    private async Task<User?> ResolveUserAsync(Guid userGuid)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.GUserId == userGuid);
    }

    public async Task<bool> AddMemberAsync(Guid projectId, AddMemberRequest request)
    {
        var project = await ResolveProjectAsync(projectId);
        if (project == null)
            return false;

        var adminProject = await _auth.GetProjectIfAdminAsync(projectId);
        if (adminProject == null)
            return false;

        var email = request.Email.Trim().ToLower();

        var invitedUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (invitedUser == null)
            return false;

        var exists = await _db.ProjectUsers.AnyAsync(pu =>
            pu.ProjectId == project.ProjectId &&
            pu.UserId == invitedUser.UserId);

        if (exists)
            return false;

        _db.ProjectUsers.Add(new ProjectUser
        {
            ProjectId = project.ProjectId,
            UserId = invitedUser.UserId,
            Role = request.Role,
            AddedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EditMemberStatus(Guid projectId, Guid userGuid, EditMemberRequest request)
    {
        var project = await ResolveProjectAsync(projectId);
        if (project == null)
            return false;

        var adminProject = await _auth.GetProjectIfAdminAsync(projectId);
        if (adminProject == null)
            return false;

        var user = await ResolveUserAsync(userGuid);
        if (user == null)
            return false;

        var membership = await _db.ProjectUsers.FirstOrDefaultAsync(pu =>
            pu.ProjectId == project.ProjectId &&
            pu.UserId == user.UserId);

        if (membership == null)
            return false;

        membership.Role = request.Role;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveMemberStatusAsync(Guid projectId, Guid userGuid)
    {
        var requesterId = _currentUser.UserId;

        var project = await ResolveProjectAsync(projectId);
        if (project == null)
            return false;

        var adminProject = await _auth.GetProjectIfAdminAsync(projectId);
        if (adminProject == null)
            return false;

        var user = await ResolveUserAsync(userGuid);
        if (user == null)
            return false;

        var requester = await _db.ProjectUsers.FirstOrDefaultAsync(x =>
            x.ProjectId == project.ProjectId &&
            x.UserId == requesterId);

        if (requester == null)
            return false;

        var target = await _db.ProjectUsers.FirstOrDefaultAsync(x =>
            x.ProjectId == project.ProjectId &&
            x.UserId == user.UserId);

        if (target == null)
            return false;

        if (requester.UserId == target.UserId)
            return false;

        var isOwner = project.ProjectOwnerId == requesterId;

        var canRemove =
            isOwner ||
            (requester.Role == "Admin" && target.Role == "Member");

        if (!canRemove)
            return false;

        _db.ProjectUsers.Remove(target);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LeaveProjectAsync(Guid projectId)
    {
        var userId = _currentUser.UserId;

        var project = await ResolveProjectAsync(projectId);
        if (project == null)
            return false;

        if (userId == project.ProjectOwnerId)
            return false;

        var member = await _db.ProjectUsers.FirstOrDefaultAsync(pu =>
            pu.ProjectId == project.ProjectId &&
            pu.UserId == userId);

        if (member == null)
            return false;

        _db.ProjectUsers.Remove(member);
        await _db.SaveChangesAsync();

        return true;
    }
}