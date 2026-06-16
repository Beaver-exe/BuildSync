namespace BuildSync.Models;

public class Document
{
    public int DocumentId { get; set; }
    public Guid GDocumentId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int UploadedByUserId { get; set; }
    public User UploadedByUser { get; set; } = null!;
    public int ProjectCategoryId { get; set; }
    public ProjectCategory ProjectCategory { get; set; } = null!;
}