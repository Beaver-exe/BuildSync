namespace BuildSync.DTOs.Category;

public class CategoryDocument
{
    public Guid GDocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public long FileSize { get; set; }
}