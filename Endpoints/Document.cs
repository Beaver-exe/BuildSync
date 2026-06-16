using BuildSync.DTOs.Document;
using BuildSync.Services;

namespace BuildSync.Endpoints;

public static class Documents
{
    public static void MapDocumentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/projects/{projectId}/categories/{categoryId}/documents").RequireAuthorization();

        group.MapGet("/{documentId}", 
        async (
            DocumentService docu, 
            Guid projectId, 
            Guid categoryId, 
            Guid documentId) =>
        {
            var document = await docu.GetDocumentAsync(projectId, categoryId, documentId);

            if (document == null)
            {
                return Results.BadRequest();
            }

            return Results.File(
                document.FilePath,
                document.ContentType,
                document.FileName
            );
        });
    }
}