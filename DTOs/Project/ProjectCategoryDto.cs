using BuildSync.DTOs.Document;

namespace BuildSync.DTOs.Project;

public class ProjectCategoryDto
{
    public int ProjectCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<DocumentDto> Documents { get; set; } = null!;
}