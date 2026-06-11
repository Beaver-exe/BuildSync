using BuildSync.DTOs.Category;
using BuildSync.Services;

namespace BuildSync.Endpoints;

public static class Categories
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/category").RequireAuthorization();

        app.MapPost("/{projectId}/category", async (CategoryService cate, int projectId, CreateCategoryRequest request) =>
        {
            var newCategory = await cate.CreateCategoryAsync(projectId, request);

            if (newCategory == null)
            {
                return Results.BadRequest("Failed to create new category");
            }

            return Results.Ok(newCategory);
        });

    }
}