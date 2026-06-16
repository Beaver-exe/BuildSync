
using BuildSync.Data;
using BuildSync.DTOs.Category;
using BuildSync.DTOs.Document;
using BuildSync.Mappings;
using BuildSync.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildSync.Services;

public class DocumentService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ProjectAuthorizationService _auth;

    public DocumentService(AppDbContext db, ICurrentUserService currentUser, ProjectAuthorizationService auth)
    {
        _db = db;
        _currentUser = currentUser;
        _auth = auth;
    }

    public async Task<Document?> GetDocumentAsync(Guid projectId, Guid categoryId, Guid documentId)
    {
        var isMember = await _auth.CanAccessProjectCategoryAsync(projectId, categoryId);

        if (!isMember) {
            return null;
        }

        var document = await _db.Documents
            .FirstOrDefaultAsync(d => d.GDocumentId == documentId);

        if (document == null){
            return null;
        }

        return document;
    }

    public async Task<CategoryDocument?> UploadDocumentAsync(Guid projectId, Guid categoryId, UploadDocumentRequest request)
    {
        var isMember = await _auth.CanAccessProjectCategoryAsync(projectId, categoryId);

        if (!isMember)
        {
            return null;
        }

        var category = await _db.ProjectCategories.FirstOrDefaultAsync(pc => pc.GCategoryId == categoryId);

        if (category == null)
        {
            return null;
        }

        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream);

        var document = new Document
        {
            GDocumentId = Guid.NewGuid(),
            FileName = request.File.FileName,
            ContentType = request.File.ContentType,
            FileSize = request.File.Length,
            Data = memoryStream.ToArray(),

            UploadedByUserId = _currentUser.UserId,
            ProjectCategoryId = category.ProjectCategoryId
        };

        _db.Documents.Add(document);
        await _db.SaveChangesAsync();
        
        return DocumentMapper.ToCategoryDocument(document);
    }

}
