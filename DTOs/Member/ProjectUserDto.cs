namespace BuildSync.DTOs.Member;

public class ProjectUserDto
{
    public Guid GUserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
}