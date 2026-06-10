using BuildSync.Data;
using BuildSync.DTOs.Project;
using BuildSync.DTOs.Users;
using BuildSync.Models;
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
            
            Projects = projects.Select(p => new GetProjectsDto
            {
                ProjectId = p.ProjectId,
                ProjectName = p.ProjectName,
                ProjectDescription = p.ProjectDescription,
                IsOwner = p.ProjectOwnerId == userId,
                IsAdmin = p.ProjectUsers.Any(u =>
                    u.UserId == userId && u.Role == "Admin")

            }).ToList()
        };

        return response;
    }

    public async Task<ProjectDto?> RetrieveProjectAsync(int ProjectId)
    {
        return null;
    }


    public async Task<ProjectDto> CreateProjectAsync(CreateProjectRequest request)
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

        var dto = new ProjectDto
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            ProjectDescription = project.ProjectDescription,
            ProjectOwnerId = project.ProjectOwnerId,
            ProjectOwner = new UserDto
            {
                UserId = project.ProjectOwner.UserId,
                FirstName = project.ProjectOwner.FirstName,
                LastName = project.ProjectOwner.LastName,
                Profession = project.ProjectOwner.Profession,
                Email = project.ProjectOwner.Email
            },
            ProjectUsers = project.ProjectUsers.Select(pu => new ProjectUserDto
            {
                UserId = pu.UserId,
                Role = pu.Role,
                AddedAt = pu.AddedAt
            }).ToList(),
            Categories = []
        };

        return dto;
    }
}