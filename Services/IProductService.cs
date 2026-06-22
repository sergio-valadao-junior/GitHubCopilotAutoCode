using GitHubCopilotAutoCode.Common;
using GitHubCopilotAutoCode.Endpoints;

namespace GitHubCopilotAutoCode.Services;

public interface IProductService
{
    Task<PagedResult<ProductWithCategoryResponse>> GetAllAsync(PaginationOptions options);
    Task<ProductWithCategoryResponse?> GetByIdAsync(Guid id);
    Task<ProductWithCategoryResponse> CreateAsync(CreateProductRequest request);
    Task<ProductWithCategoryResponse?> UpdateAsync(Guid id, UpdateProductRequest request);
    Task<bool> DeleteAsync(Guid id);
}
