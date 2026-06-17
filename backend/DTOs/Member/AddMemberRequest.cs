namespace BuildSync.DTOs.Member;

public class AddMemberRequest
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
}