using BuildSync.DTOs.Users;
using BuildSync.DTOs.Member;
using BuildSync.DTOs.Category;

namespace BuildSync.DTOs.Project;

public class ProjectDto
{
    public Guid GProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectDescription { get; set; } = string.Empty;
    public int ProjectOwnerId { get; set; }
    public UserDto ProjectOwner { get; set; } = null!;
    public List<ProjectUserDto> ProjectUsers { get; set; } = new();
    public List<ProjectCategoryDto> Categories { get; set; } = new();
}