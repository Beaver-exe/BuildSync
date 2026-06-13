using BuildSync.DTOs.Category;
using BuildSync.Data;
using BuildSync.Models;
using BuildSync.Mappings;
using Microsoft.EntityFrameworkCore;

namespace BuildSync.Services;

public class CategoryService
{
    private readonly AppDbContext _db;
    private readonly ProjectAuthorizationService _auth;

    public CategoryService(AppDbContext db, ProjectAuthorizationService auth)
    {
        _db = db;
        _auth = auth;
    }

    public async Task<ProjectCategoryDto?> CreateCategoryAsync(Guid projectId, CreateCategoryRequest request)
    {
        var project = await _auth.GetProjectIfAdminAsync(projectId);

        if (project == null)
        {
            return null;
        }

        var category = new ProjectCategory
        {
            CategoryName = request.CategoryName,
            ProjectId = project.ProjectId
        };

        _db.ProjectCategories.Add(category);
        await _db.SaveChangesAsync();

        return new ProjectCategoryDto
        {
            ProjectCategoryId = category.ProjectCategoryId,
            CategoryName = category.CategoryName
        };
    }

    public async Task<bool> DeleteCategoryAsync(Guid projectId, int categoryId)
    {
        var project = await _auth.GetProjectIfAdminAsync(projectId);

        if (project == null)
        {
            return false;
        }

        var category = await _db.ProjectCategories
            .FirstOrDefaultAsync(c =>
                c.ProjectCategoryId == categoryId &&
                c.ProjectId == project.ProjectId);

        if (category == null)
        {
            return false;
        }

        _db.ProjectCategories.Remove(category);
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<List<CategoryDocument>?> FetchCategoryDocumentsAsync(Guid projectId, int categoryId)
    {
        var project = await _auth.GetProjectIfMemberAsync(projectId);

        if (project == null)
        {
            return null;
        }

        var documents = await _db.Documents
            .Where(d =>
                d.ProjectCategoryId == categoryId &&
                d.ProjectCategory.ProjectId == project.ProjectId)
            .ToListAsync();

        return DocumentMapper.ToCategoryDocuments(documents);
    }
}