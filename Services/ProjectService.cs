using BuildSync.Data;
using BuildSync.DTOs.Project;
using BuildSync.Models;
using BuildSync.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BuildSync.Services;

public class ProjectService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ProjectAuthorizationService _auth;

    public ProjectService(AppDbContext db, ICurrentUserService currentUser, ProjectAuthorizationService auth)
    {
        _db = db;
        _currentUser = currentUser;
        _auth = auth;
    }

    public async Task<GetProjectsResponse> RetrieveProjectsAsync()
    {
        var userId = _currentUser.UserId;

        var projects = await _db.Projects
            .Include(p => p.ProjectUsers)
            .Where(p =>
                p.ProjectOwnerId == userId ||
                p.ProjectUsers.Any(u => u.UserId == userId))
            .ToListAsync();

        return new GetProjectsResponse
        {
            Message = "Projects retrieval successful",
            Projects = projects.Select(p =>
                ProjectMapper.ToSummaryDto(p, userId)).ToList()
        };
    }

    public async Task<ProjectDto?> RetrieveProjectAsync(Guid projectId)
    {
        var project = await _auth.GetProjectIfMemberAsync(projectId);

        return project == null
            ? null
            : ProjectMapper.ToDto(project);
    }

    public async Task<GetProjectDto> CreateProjectAsync(CreateProjectRequest request)
    {
        var userId = _currentUser.UserId;

        var project = new Project
        {
            ProjectName = request.ProjectName,
            ProjectDescription = request.ProjectDescription,
            ProjectOwnerId = userId
        };

        project.ProjectUsers.Add(new ProjectUser
        {
            UserId = userId,
            Role = "Admin"
        });

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        return ProjectMapper.ToSummaryDto(project, userId);
    }

    public async Task<GetProjectDto?> EditProjectAsync(CreateProjectRequest request, Guid projectId)
    {
        var project = await _auth.GetProjectIfAdminAsync(projectId);

        if (project == null)
        {
            return null;
        }

        project.ProjectName = request.ProjectName;
        project.ProjectDescription = request.ProjectDescription;

        await _db.SaveChangesAsync();

        return ProjectMapper.ToSummaryDto(project, _currentUser.UserId);
    }

    public async Task<bool> DeleteProjectAsync(Guid projectId)
    {
        var project = await _auth.GetProjectIfAdminAsync(projectId);

        if (project == null)
        {
            return false;
        }

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        return true;
    }
}