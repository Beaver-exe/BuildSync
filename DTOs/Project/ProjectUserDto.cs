using BuildSync.DTOs.Users;

namespace BuildSync.DTOs.Project;

public class ProjectUserDto
{
    public int UserId { get; set; }
    public UserDto User { get; set; } = null!;
    public string Role { get; set; } = "Member";
    public DateTime AddedAt { get; set; }
}