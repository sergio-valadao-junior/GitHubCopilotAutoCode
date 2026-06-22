using GitHubCopilotAutoCode.Common;
using GitHubCopilotAutoCode.Data;
using GitHubCopilotAutoCode.Endpoints;
using GitHubCopilotAutoCode.Models;
using Microsoft.EntityFrameworkCore;

namespace GitHubCopilotAutoCode.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<CategoryResponse>> GetAllAsync(PaginationOptions options)
    {
        var query = _context.Categories.AsQueryable();

        if (!string.IsNullOrEmpty(options.SearchTerm))
        {
            query = query.Where(c =>
                c.Title.Contains(options.SearchTerm) ||
                c.Description.Contains(options.SearchTerm));
        }

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

        return await query
            .Select(c => new CategoryResponse(
                c.Id,
                c.Title,
                c.Description,
                c.CreatedAtUtc,
                c.UpdatedAtUtc))
            .ToPagedResultAsync(options.PageNumber, options.PageSize);
    }

    public async Task<CategoryWithProductsResponse?> GetByIdAsync(Guid id)
    {
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);

        return category is null
            ? null
            : ToCategoryWithProductsResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return ToCategoryResponse(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null)
        {
            return null;
        }

        category.Title = request.Title;
        category.Description = request.Description;
        category.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ToCategoryResponse(category);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category is null)
        {
            return false;
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    private static CategoryResponse ToCategoryResponse(Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Title,
            category.Description,
            category.CreatedAtUtc,
            category.UpdatedAtUtc);
    }

    private static CategoryWithProductsResponse ToCategoryWithProductsResponse(Category category)
    {
        return new CategoryWithProductsResponse(
            category.Id,
            category.Title,
            category.Description,
            category.CreatedAtUtc,
            category.UpdatedAtUtc,
            category.Products.Select(p => new ProductSummary(p.Id, p.Name, p.Price)).ToList());
    }
}
