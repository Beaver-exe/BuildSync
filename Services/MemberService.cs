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

    private async Task<Project?> ResolveAdminProjectAsync(int projectId)
    {
        return await _auth.GetProjectIfAdminAsync(projectId);
    }

    private async Task<User?> ResolveUserAsync(Guid userGuid)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.GUserId == userGuid);
    }

    public async Task<bool> AddMemberAsync(int projectId, AddMemberRequest request)
    {
        var email = request.Email.Trim().ToLower();

        var invitedUser = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (invitedUser == null)
        {
            return false;
        }

        var project = await ResolveAdminProjectAsync(projectId);

        if (project == null)
        {
            return false;
        }

        var exists = await _db.ProjectUsers.AnyAsync(pu =>
            pu.ProjectId == projectId &&
            pu.UserId == invitedUser.UserId);

        if (exists)
        {
            return false;
        }

        var projUser = new ProjectUser
        {
            ProjectId = projectId,
            UserId = invitedUser.UserId,
            Role = request.Role,
            AddedAt = DateTime.UtcNow
        };

        _db.ProjectUsers.Add(projUser);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> EditMemberStatus(int projectId, Guid userGuid, EditMemberRequest request)
    {
        var project = await ResolveAdminProjectAsync(projectId);

        if (project == null)
        {
            return false;
        }

        var user = await ResolveUserAsync(userGuid);

        if (user == null)
        {
            return false;
        }

        var membership = await _db.ProjectUsers
            .FirstOrDefaultAsync(pu =>
                pu.ProjectId == projectId &&
                pu.UserId == user.UserId);

        if (membership == null)
        {
            return false;
        }

        membership.Role = request.Role;
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveMemberStatusAsync(int projectId, Guid userGuid)
    {
        var requesterId = _currentUser.UserId;

        var project = await ResolveAdminProjectAsync(projectId);

        if (project == null)
        {
            return false;
        }

        var user = await ResolveUserAsync(userGuid);

        if (user == null)
        {
            return false;
        }

        var requester = await _db.ProjectUsers
            .FirstOrDefaultAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == requesterId);

        if (requester == null)
        {
            return false;
        }

        var target = await _db.ProjectUsers
            .FirstOrDefaultAsync(x =>
                x.ProjectId == projectId &&
                x.UserId == user.UserId);

        if (target == null)
        {
            return false;
        }

        if (requester.UserId == target.UserId)
        {
            return false;
        }

        var isOwner = project.ProjectOwnerId == requesterId;

        var canRemove =
            isOwner ||
            (requester.Role == "Admin" && target.Role == "Member");

        if (!canRemove)
        {
            return false;
        }

        _db.ProjectUsers.Remove(target);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> LeaveProjectAsync(int projectId)
    {
        var userId = _currentUser.UserId;

        var project = await _auth.GetProjectIfMemberAsync(projectId);

        if (project == null)
        {
            return false;
        }

        if (userId == project.ProjectOwnerId)
        {
            return false;
        }

        var member = await _db.ProjectUsers.FirstOrDefaultAsync(pu =>
            pu.ProjectId == projectId &&
            pu.UserId == userId);

        if (member == null)
        {
            return false;
        }

        _db.ProjectUsers.Remove(member);
        await _db.SaveChangesAsync();

        return true;
    }
}