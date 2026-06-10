namespace BuildSync.DTOs.Project;

public class GetProjectsResponse
{  
    public string Message { get; set; } = string.Empty;
    public List<GetProjectsDto> Projects { get; set; } = null!;
}