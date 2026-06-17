namespace BuildSync.DTOs.Document;

public class EditDocumentRequest
{
    public string? FileName { get; set; }
    public IFormFile? File { get; set; }
}