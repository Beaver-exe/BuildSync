using BuildSync.DTOs.Category;
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
            UploadedAt = document.UploadedAt
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