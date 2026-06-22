using GitHubCopilotAutoCode.Common;
using GitHubCopilotAutoCode.Endpoints;

namespace GitHubCopilotAutoCode.Services;

public interface ICategoryService
{
    Task<PagedResult<CategoryResponse>> GetAllAsync(PaginationOptions options);
    Task<CategoryWithProductsResponse?> GetByIdAsync(Guid id);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task<CategoryResponse?> UpdateAsync(Guid id, UpdateCategoryRequest request);
    Task<bool> DeleteAsync(Guid id);
}
