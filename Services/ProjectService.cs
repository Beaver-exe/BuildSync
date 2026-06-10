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
            
            Projects = projects.Select(p => new GetProjectDto
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

    public async Task<ProjectDto?> RetrieveProjectAsync(int projectId)
    {  
        var userId = _currentUser.UserId;

        var project = await _db.Projects
            .Include(p => p.ProjectOwner)
            .Include(p => p.ProjectUsers)
                .ThenInclude(pu => pu.User)
            .Include(p => p.Categories)
            .Where(p => 
                p.ProjectId == projectId &&
                p.ProjectUsers.Any(u => u.UserId == userId))
            .FirstOrDefaultAsync();

        if (project == null)
        {
            return null;
        }

        var projectDto = new ProjectDto
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
                FirstName = pu.User.FirstName,
                LastName = pu.User.LastName,
                Email = pu.User.Email,
                Role = pu.Role

            }).ToList(),
            
            Categories = project.Categories.Select(c => new ProjectCategoryDto
            {
                ProjectCategoryId = c.ProjectCategoryId,
                CategoryName = c.CategoryName
            }).ToList()
        };

        return projectDto;
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

        var dto = new GetProjectDto
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            ProjectDescription = project.ProjectDescription,
            IsOwner = project.ProjectOwnerId == userId,
            IsAdmin = project.ProjectUsers.Any(u => u.UserId == userId && u.Role == "Admin")
        };

        return dto;
    }
}