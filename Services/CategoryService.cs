using BuildSync.DTOs.Category;
using BuildSync.Data;
using BuildSync.Models;
using Microsoft.EntityFrameworkCore;

namespace BuildSync.Services;

public class CategoryService
{
    private readonly AppDbContext _db;
    private readonly CurrentUserService _currentUser;

    public CategoryService(AppDbContext db, CurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ProjectCategoryDto?> CreateCategoryAsync(int projectId, CreateCategoryRequest request)
    {
        int userId = _currentUser.UserId;
        
        var project = await _db.Projects
        .Include(p => p.ProjectUsers)
        .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        
        if (project == null)
        {
            return null;
        }

        var membership = project.ProjectUsers.FirstOrDefault(pu => pu.UserId == userId);

        if (membership?.Role != "Admin")
        {
            return null;
        }

        var category = new ProjectCategory
        {
          CategoryName = request.CategoryName,
          ProjectId = projectId  
        };

        _db.ProjectCategories.Add(category);
        await _db.SaveChangesAsync();

        return new ProjectCategoryDto
        {
            ProjectCategoryId = category.ProjectCategoryId,
            CategoryName = category.CategoryName
        };
    }
    
}