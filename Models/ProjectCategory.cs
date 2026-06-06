namespace BuildSync.Models;

public class ProjectCategory
{
    public int ProjectCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public string ProjectName { get; set; } = string.Empty;
    public List<Document> Documents { get; set; } = new();
}