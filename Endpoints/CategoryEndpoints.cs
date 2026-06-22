using GitHubCopilotAutoCode.Common;
using GitHubCopilotAutoCode.Endpoints;
using GitHubCopilotAutoCode.Models;
using GitHubCopilotAutoCode.Services;
using Microsoft.AspNetCore.Http;

namespace GitHubCopilotAutoCode.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/categories")
            .WithName("Categories");

        group.MapGet("/", GetAllCategories)
            .WithName("GetAllCategories")
            .WithSummary("Get all categories with pagination and filtering")
            .Produces<PagedResult<CategoryResponse>>(200);

        group.MapGet("/{id}", GetCategoryById)
            .WithName("GetCategoryById")
            .WithSummary("Get category by ID");

        group.MapPost("/", CreateCategory)
            .WithName("CreateCategory")
            .WithSummary("Create a new category");

        group.MapPut("/{id}", UpdateCategory)
            .WithName("UpdateCategory")
            .WithSummary("Update an existing category");

        group.MapDelete("/{id}", DeleteCategory)
            .WithName("DeleteCategory")
            .WithSummary("Delete a category");
    }

    private static async Task<IResult> GetAllCategories(
        [AsParameters] PaginationOptions options,
        ICategoryService categoryService)
    {
        var pagedResult = await categoryService.GetAllAsync(options);
        return Results.Ok(pagedResult);
    }

    private static async Task<IResult> GetCategoryById(Guid id, ICategoryService categoryService)
    {
        var category = await categoryService.GetByIdAsync(id);
        return category is null ? Results.NotFound() : Results.Ok(category);
    }

    private static async Task<IResult> CreateCategory(CreateCategoryRequest request, ICategoryService categoryService)
    {
        var response = await categoryService.CreateAsync(request);
        return Results.Created($"/api/categories/{response.Id}", response);
    }

    private static async Task<IResult> UpdateCategory(Guid id, UpdateCategoryRequest request, ICategoryService categoryService)
    {
        var response = await categoryService.UpdateAsync(id, request);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }

    private static async Task<IResult> DeleteCategory(Guid id, ICategoryService categoryService)
    {
        var deleted = await categoryService.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static CategoryResponse ToCategoryResponse(Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Title,
            category.Description,
            category.CreatedAtUtc,
            category.UpdatedAtUtc
        );
    }

    private static CategoryWithProductsResponse ToCategoryWithProductsResponse(Category category)
    {
        return new CategoryWithProductsResponse(
            category.Id,
            category.Title,
            category.Description,
            category.CreatedAtUtc,
            category.UpdatedAtUtc,
            category.Products.Select(p => new ProductSummary(p.Id, p.Name, p.Price)).ToList()
        );
    }
}

public sealed record CreateCategoryRequest(string Title, string Description);

public sealed record UpdateCategoryRequest(string Title, string Description);

public sealed record ProductSummary(Guid Id, string Name, decimal Price);

public sealed record CategoryResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);

public sealed record CategoryWithProductsResponse(
    Guid Id,
    string Title,
    string Description,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    List<ProductSummary> Products
);
