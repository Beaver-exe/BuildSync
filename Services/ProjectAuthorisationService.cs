using BuildSync.Data;
using BuildSync.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildSync.Services;

public class ProjectAuthorizationService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ProjectAuthorizationService(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Project?> GetProjectIfAdminAsync(Guid projectId)
    {
        var userId = _currentUser.UserId;

        return await _db.Projects
            .Include(p => p.ProjectUsers)
            .FirstOrDefaultAsync(p =>
                p.GProjectId == projectId &&
                p.ProjectUsers.Any(pu =>
                    pu.UserId == userId &&
                    pu.Role == "Admin"));
    }

    public async Task<Project?> GetProjectIfMemberAsync(Guid projectId)
    {
        var userId = _currentUser.UserId;

        return await _db.Projects
            .Include(p => p.ProjectUsers)
            .FirstOrDefaultAsync(p =>
                p.GProjectId == projectId &&
                p.ProjectUsers.Any(pu =>
                    pu.UserId == userId));
    }

    public bool IsAdmin(Project project)
    {
        var userId = _currentUser.UserId;

        return project.ProjectUsers.Any(pu =>
            pu.UserId == userId &&
            pu.Role == "Admin");
    }

    public bool IsMember(Project project)
    {
        var userId = _currentUser.UserId;

        return project.ProjectUsers.Any(pu =>
            pu.UserId == userId);
    }
}