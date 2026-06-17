namespace BuildSync.DTOs.Project;

public class GetProjectsResponse
{  
    public string Message { get; set; } = string.Empty;
    public List<GetProjectDto> Projects { get; set; } = null!;
}