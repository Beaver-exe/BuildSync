using BuildSync.DTOs.Category;
using BuildSync.DTOs.Users;
using BuildSync.Models;

namespace BuildSync.Mappings;

public static class DocumentMapper
{
    public static CategoryDocument ToCategoryDocument(Document document)
    {
        return new CategoryDocument
        {
            GDocumentId = document.GDocumentId,
            FileName = document.FileName,
            UploadedAt = document.UploadedAt,
            FileSize = document.FileSize,
            UploadedByUser = new UploadedByUser
            {
                GUserId = document.UploadedByUser.GUserId,
                FirstName = document.UploadedByUser.FirstName,
                LastName = document.UploadedByUser.LastName,
                Profession = document.UploadedByUser.Profession
            }
        };
    }

    public static List<CategoryDocument> ToCategoryDocuments(
        IEnumerable<Document> documents)
    {
        return documents
            .Select(ToCategoryDocument)
            .ToList();
    }
}