namespace BuildSync.DTOs.Document;

public class UploadDocumentRequest
{
    public string? FileName { get; set; }
    public IFormFile File { get; set; } = null!;
}