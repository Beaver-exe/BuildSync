namespace BuildSync.DTOs.Users;

public class UploadedByUser
{
    public Guid GUserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Profession { get; set; } = string.Empty;
}