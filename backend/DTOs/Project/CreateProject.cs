namespace BuildSync.DTOs.Project;

public class CreateProjectRequest
{
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectDescription { get; set; } = string.Empty;
}