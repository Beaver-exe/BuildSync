
using BuildSync.Data;
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
}
