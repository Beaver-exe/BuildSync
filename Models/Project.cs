namespace BuildSync.Models;

public class Project
{
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectDescription { get; set; } = string.Empty;
    public int ProjectOwnerId { get; set; }
    public User ProjectOwner { get; set; } = null!;
    public List<ProjectUser> ProjectUsers { get; set; } = new();
    public List<ProjectCategory> Categories { get; set; } = new();
}