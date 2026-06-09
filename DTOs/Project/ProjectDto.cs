using BuildSync.DTOs.Users;

namespace BuildSync.DTOs.Project;

public class ProjectDto
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectDescription { get; set; } = string.Empty;
    public int ProjectOwnerId { get; set; }
    public UserDto ProjectOwner { get; set; } = null!;
    public List<ProjectUserDto> ProjectUsers { get; set; } = new();
    public List<ProjectCategoryDto> Categories { get; set; } = new();
}