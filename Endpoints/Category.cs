using BuildSync.DTOs.Category;
using BuildSync.Services;

namespace BuildSync.Endpoints;

public static class Categories
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/projects/{projectsId}/categories").RequireAuthorization();

        group.MapPost("/", async (CategoryService cate, int projectId, CreateCategoryRequest request) =>
        {
            var newCategory = await cate.CreateCategoryAsync(projectId, request);

            if (newCategory == null)
            {
                return Results.BadRequest("Failed to create new category");
            }

            return Results.Ok(newCategory);
        });

        group.MapGet("/{categoryId}", async (CategoryService cate, int projectId, int categoryId) =>
        {
            var documents = await cate.FetchCategoryDocumentsAsync(projectId, categoryId);

            return Results.Ok(documents);
        });

        group.MapDelete("/{categoryId}", async (CategoryService cate, int projectId, int categoryId) =>
        {
            var sucess = await cate.DeleteCategoryAsync(projectId, categoryId);

            if (!sucess)
            {
                return Results.BadRequest("Failed to delete category");
            }

            return Results.Ok();
        });
    }
}