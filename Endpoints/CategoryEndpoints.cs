using GitHubCopilotAutoCode.Common;
using GitHubCopilotAutoCode.Data;
using GitHubCopilotAutoCode.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
        ApplicationDbContext context)
    {
        var query = context.Categories.AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrEmpty(options.SearchTerm))
        {
            query = query.Where(c =>
                c.Title.Contains(options.SearchTerm) ||
                c.Description.Contains(options.SearchTerm));
        }

        // Apply sorting
        query = options.SortBy?.ToLower() switch
        {
            "title" => options.SortDirection == "desc"
                ? query.OrderByDescending(c => c.Title)
                : query.OrderBy(c => c.Title),
            "createdat" => options.SortDirection == "desc"
                ? query.OrderByDescending(c => c.CreatedAtUtc)
                : query.OrderBy(c => c.CreatedAtUtc),
            _ => query.OrderBy(c => c.CreatedAtUtc)
        };

        var pagedResult = await query
            .Select(c => new CategoryResponse(
                c.Id,
                c.Title,
                c.Description,
                c.CreatedAtUtc,
                c.UpdatedAtUtc))
            .ToPagedResultAsync(options.PageNumber, options.PageSize);

        return Results.Ok(pagedResult);
    }

    private static async Task<IResult> GetCategoryById(Guid id, ApplicationDbContext context)
    {
        var category = await context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
        
        if (category is null)
            return Results.NotFound();

        var response = ToCategoryWithProductsResponse(category);
        return Results.Ok(response);
    }

    private static async Task<IResult> CreateCategory(CreateCategoryRequest request, ApplicationDbContext context)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var response = ToCategoryResponse(category);
        return Results.Created($"/api/categories/{category.Id}", response);
    }

    private static async Task<IResult> UpdateCategory(Guid id, UpdateCategoryRequest request, ApplicationDbContext context)
    {
        var category = await context.Categories.FindAsync(id);
        if (category is null)
            return Results.NotFound();

        category.Title = request.Title;
        category.Description = request.Description;
        category.UpdatedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync();

        var response = ToCategoryResponse(category);
        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteCategory(Guid id, ApplicationDbContext context)
    {
        var category = await context.Categories.FindAsync(id);
        if (category is null)
            return Results.NotFound();

        context.Categories.Remove(category);
        await context.SaveChangesAsync();

        return Results.NoContent();
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
