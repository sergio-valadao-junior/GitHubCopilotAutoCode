using GitHubCopilotAutoCode.Common;
using GitHubCopilotAutoCode.Endpoints;
using GitHubCopilotAutoCode.Models;
using GitHubCopilotAutoCode.Services;
using Microsoft.AspNetCore.Http;

namespace GitHubCopilotAutoCode.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products")
            .WithName("Products");

        group.MapGet("/", GetAllProducts)
            .WithName("GetAllProducts")
            .WithSummary("Get all products with pagination and filtering")
            .Produces<PagedResult<ProductWithCategoryResponse>>(200);

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

    private static async Task<IResult> GetAllProducts(
        [AsParameters] PaginationOptions options,
        IProductService productService)
    {
        var pagedResult = await productService.GetAllAsync(options);
        return Results.Ok(pagedResult);
    }

    private static async Task<IResult> GetProductById(Guid id, IProductService productService)
    {
        var product = await productService.GetByIdAsync(id);

        return product is null ? Results.NotFound() : Results.Ok(product);
    }

    private static async Task<IResult> CreateProduct(CreateProductRequest request, IProductService productService)
    {
        var response = await productService.CreateAsync(request);
        return Results.Created($"/api/products/{response.Id}", response);
    }

    private static async Task<IResult> UpdateProduct(Guid id, UpdateProductRequest request, IProductService productService)
    {
        var response = await productService.UpdateAsync(id, request);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }

    private static async Task<IResult> DeleteProduct(Guid id, IProductService productService)
    {
        var deleted = await productService.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound();
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
