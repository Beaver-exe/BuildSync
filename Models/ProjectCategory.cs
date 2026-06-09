namespace BuildSync.Models;

public class ProjectCategory
{
    public int ProjectCategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public List<Document> Documents { get; set; } = new();
}