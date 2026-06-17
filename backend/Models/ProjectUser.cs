namespace BuildSync.Models;

public class ProjectUser
{
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string Role { get; set; } = "Member";
    public DateTime AddedAt { get; set; }
}