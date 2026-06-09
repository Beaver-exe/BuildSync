using BuildSync.DTOs.Users;

namespace BuildSync.DTOs.Document;

public class DocumentDto
{
    public int DocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public UserDto UploadedByUser { get; set; } = null!;
}