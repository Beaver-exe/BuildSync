using BuildSync.DTOs.Project;
using BuildSync.DTOs.Category;
using BuildSync.DTOs.Users;
using BuildSync.DTOs.Member;
using BuildSync.Models;

namespace BuildSync.Mappings;

public static class ProjectMapper
{
    public static ProjectDto ToDto(Project project)
    {
        return new ProjectDto
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
    }

    public static GetProjectDto ToSummaryDto(Project project, int currentUserId)
    {
        return new GetProjectDto
        {
            ProjectId = project.ProjectId,
            ProjectName = project.ProjectName,
            ProjectDescription = project.ProjectDescription,
            IsOwner = project.ProjectOwnerId == currentUserId,
            IsAdmin = project.ProjectUsers.Any(u => u.UserId == currentUserId && u.Role == "Admin")
        };
    }
}