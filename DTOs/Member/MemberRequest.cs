namespace BuildSync.DTOs.Member;

public class MemberRequest
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
}