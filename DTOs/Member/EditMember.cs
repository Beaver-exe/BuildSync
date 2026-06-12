namespace BuildSync.DTOs.Member;

public class EditMemberRequest
{
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Member";
}