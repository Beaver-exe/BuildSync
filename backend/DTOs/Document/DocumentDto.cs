using BuildSync.DTOs.Users;

namespace BuildSync.DTOs.Document;

public class DocumentDto
{
    public Guid GDocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public UserDto UploadedByUser { get; set; } = null!;
}