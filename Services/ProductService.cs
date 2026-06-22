using GitHubCopilotAutoCode.Common;
using GitHubCopilotAutoCode.Data;
using GitHubCopilotAutoCode.Endpoints;
using GitHubCopilotAutoCode.Models;
using Microsoft.EntityFrameworkCore;

namespace GitHubCopilotAutoCode.Services;

public sealed class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<ProductWithCategoryResponse>> GetAllAsync(PaginationOptions options)
    {
        var query = _context.Products.Include(p => p.Category).AsQueryable();

        if (!string.IsNullOrEmpty(options.SearchTerm))
        {
            query = query.Where(p =>
                p.Name.Contains(options.SearchTerm) ||
                p.Description.Contains(options.SearchTerm));
        }

        query = options.SortBy?.ToLower() switch
        {
            "name" => options.SortDirection == "desc"
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            "price" => options.SortDirection == "desc"
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            "createdat" => options.SortDirection == "desc"
                ? query.OrderByDescending(p => p.CreatedAtUtc)
                : query.OrderBy(p => p.CreatedAtUtc),
            _ => query.OrderBy(p => p.CreatedAtUtc)
        };

        return await query
            .Select(p => new ProductWithCategoryResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.CategoryId,
                p.CreatedAtUtc,
                p.UpdatedAtUtc,
                p.Category != null
                    ? new CategorySummary(p.Category.Id, p.Category.Title)
                    : null))
            .ToPagedResultAsync(options.PageNumber, options.PageSize);
    }

    public async Task<ProductWithCategoryResponse?> GetByIdAsync(Guid id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        return product is null
            ? null
            : ToProductWithCategoryResponse(product);
    }

    public async Task<ProductWithCategoryResponse> CreateAsync(CreateProductRequest request)
    {
        var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
        if (!categoryExists)
        {
            throw new InvalidOperationException("Category not found.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        await _context.Entry(product).Reference(p => p.Category).LoadAsync();

        return ToProductWithCategoryResponse(product);
    }

    public async Task<ProductWithCategoryResponse?> UpdateAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return null;
        }

        if (product.CategoryId != request.CategoryId)
        {
            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category is null)
            {
                throw new InvalidOperationException("Category not found.");
            }

            product.Category = category;
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ToProductWithCategoryResponse(product);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null)
        {
            return false;
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    private static ProductWithCategoryResponse ToProductWithCategoryResponse(Product product)
    {
        return new ProductWithCategoryResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.CategoryId,
            product.CreatedAtUtc,
            product.UpdatedAtUtc,
            product.Category is not null
                ? new CategorySummary(product.Category.Id, product.Category.Title)
                : null);
    }
}
