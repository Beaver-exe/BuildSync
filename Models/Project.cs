namespace BuildSync.Models;

public class Project
{
    public int ProjectId { get; set; }
    public string ProjectName {get; set; } = string.Empty;
    public int ProjectOwnerId { get; set; }
    public User ProjectOwner { get; set; } = null!;
    public List<User> Users { get; set; } = new();
    public List<ProjectCategory> Categories { get; set; } = new();
}