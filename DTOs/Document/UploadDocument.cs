namespace BuildSync.DTOs.Document;

public class UploadDocumentRequest
{
    public IFormFile File { get; set; } = null!;
}