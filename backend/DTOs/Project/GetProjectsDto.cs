namespace BuildSync.DTOs.Project;

public class GetProjectDto
{
    public Guid GProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectDescription { get; set; } = string.Empty;
    public bool IsOwner { get; set; } = false;
    public bool IsAdmin { get; set; } = false;
}