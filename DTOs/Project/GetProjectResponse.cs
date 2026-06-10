namespace BuildSync.DTOs.Project;

public class GetProjectResponse
{  
    public string Message { get; set; } = string.Empty;
    public List<GetProjectDto> Projects { get; set; } = null!;
}