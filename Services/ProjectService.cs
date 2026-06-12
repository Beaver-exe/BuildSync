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

    public ProjectService(AppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    private async Task<Project?> GetProjectWithUsersAsync(int projectId)
    {
        return await _db.Projects
            .Include(p => p.ProjectUsers)
            .FirstOrDefaultAsync(p => p.ProjectId == projectId);
    }

    private bool IsAdmin(Project project)
    {
        var userId = _currentUser.UserId;

        return project.ProjectUsers.Any(pu =>
            pu.UserId == userId &&
            pu.Role == "Admin");
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

        var response = new GetProjectsResponse
        {   
            Message = "Projects retrieval successful",
            Projects = projects.Select(p => ProjectMapper.ToSummaryDto(p, userId)).ToList()
        };

        return response;
    }

    public async Task<ProjectDto?> RetrieveProjectAsync(int projectId)
    {
        var userId = _currentUser.UserId;

        var project = await _db.Projects
            .Include(p => p.ProjectOwner)
            .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p =>
                p.ProjectId == projectId &&
                p.ProjectUsers.Any(u => u.UserId == userId));

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

    public async Task<GetProjectDto?> EditProjectAsync(CreateProjectRequest request, int projectId)
    {   
        var project = await GetProjectWithUsersAsync(projectId);

        if (project == null)
        {
            return null;
        }

        var admin = IsAdmin(project);

        if (!admin)
        {
            return null;
        }
        
        project.ProjectName = request.ProjectName;
        project.ProjectDescription = request.ProjectDescription;

        await _db.SaveChangesAsync();
       
        return ProjectMapper.ToSummaryDto(project, _currentUser.UserId);
    }
    
    public async Task<bool> DeleteProjectAsync(int projectId)
    {   
        var project = await GetProjectWithUsersAsync(projectId);

        if (project == null)
        {
            return false;
        }

        var admin = IsAdmin(project);

        if (!admin)
        {
            return false;
        }

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        return true;
    }
}