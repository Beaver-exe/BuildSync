using BuildSync.DTOs.Document;
using BuildSync.Services;

namespace BuildSync.Endpoints;

public static class Documents
{
    public static void MapDocumentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/projects/{projectId}/categories/{categoryId}/documents")
            .WithTags("Documents")
            .RequireAuthorization();

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
                return Results.NotFound();
            }

            return Results.File(
                document.Data,
                document.ContentType,
                document.FileName
            );
        });

        group.MapPost("/", 
        async (
            DocumentService docu,
            Guid projectId,
            Guid categoryId,
            UploadDocumentRequest request) =>
        {
            var document = await docu.UploadDocumentAsync(projectId, categoryId, request);

            if (document == null)
            {
                return Results.BadRequest("Failed to upload document");
            }

            return Results.Ok(document);

        });

        group.MapDelete("/{documentId}", 
        async (
            DocumentService docu, 
            Guid projectId, 
            Guid categoryId, 
            Guid documentId) =>
        {
            var success = await docu.DeleteDocumentAsync(projectId, categoryId, documentId);

            if (!success)
            {
                return Results.BadRequest();
            }

            return Results.Ok();
        });

        group.MapPatch("/{documentId}", 
        async (
            DocumentService docu, 
            Guid projectId, 
            Guid categoryId, 
            Guid documentId,
            EditDocumentRequest request) =>
        {
            var document = await docu.EditDocumentAsync(projectId, categoryId, documentId, request);

            if (document == null)
            {
                return Results.BadRequest();
            }

            return Results.Ok(document);
        });
    }
}