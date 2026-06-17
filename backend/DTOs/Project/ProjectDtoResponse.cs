namespace BuildSync.DTOs.Project;

public class ProjectDtoResponse
{
    public string Message { get; set; } = string.Empty;
    public ProjectDto Project { get; set; } = null!;
}