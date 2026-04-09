using GitHubCopilotAutoCode.Data;
using GitHubCopilotAutoCode.Models;
using Microsoft.EntityFrameworkCore;

namespace GitHubCopilotAutoCode.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products")
            .WithName("Products");

        group.MapGet("/", GetAllProducts)
            .WithName("GetAllProducts")
            .WithSummary("Get all products");

        group.MapGet("/{id}", GetProductById)
            .WithName("GetProductById")
            .WithSummary("Get product by ID");

        group.MapPost("/", CreateProduct)
            .WithName("CreateProduct")
            .WithSummary("Create a new product");

        group.MapPut("/{id}", UpdateProduct)
            .WithName("UpdateProduct")
            .WithSummary("Update an existing product");

        group.MapDelete("/{id}", DeleteProduct)
            .WithName("DeleteProduct")
            .WithSummary("Delete a product");
    }

    private static async Task<IResult> GetAllProducts(ApplicationDbContext context)
    {
        var products = await context.Products
            .Include(p => p.Category)
            .ToListAsync();
        
        var response = products.Select(ToProductWithCategoryResponse).ToList();
        return Results.Ok(response);
    }

    private static async Task<IResult> GetProductById(Guid id, ApplicationDbContext context)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
            return Results.NotFound();

        var response = ToProductWithCategoryResponse(product);
        return Results.Ok(response);
    }

    private static async Task<IResult> CreateProduct(CreateProductRequest request, ApplicationDbContext context)
    {
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

        context.Products.Add(product);
        await context.SaveChangesAsync();

        // Load the category for the response
        await context.Entry(product).Reference(p => p.Category).LoadAsync();
        var response = ToProductWithCategoryResponse(product);
        return Results.Created($"/api/products/{product.Id}", response);
    }

    private static async Task<IResult> UpdateProduct(Guid id, UpdateProductRequest request, ApplicationDbContext context)
    {
        var product = await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (product is null)
            return Results.NotFound();

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.CategoryId = request.CategoryId;
        product.UpdatedAtUtc = DateTime.UtcNow;

        await context.SaveChangesAsync();

        var response = ToProductWithCategoryResponse(product);
        return Results.Ok(response);
    }

    private static async Task<IResult> DeleteProduct(Guid id, ApplicationDbContext context)
    {
        var product = await context.Products.FindAsync(id);
        if (product is null)
            return Results.NotFound();

        context.Products.Remove(product);
        await context.SaveChangesAsync();

        return Results.NoContent();
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
                : null
        );
    }
}

public sealed record CreateProductRequest(string Name, string Description, decimal Price, Guid CategoryId);

public sealed record UpdateProductRequest(string Name, string Description, decimal Price, Guid CategoryId);

public sealed record CategorySummary(Guid Id, string Title);

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc
);

public sealed record ProductWithCategoryResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    Guid CategoryId,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    CategorySummary? Category
);
