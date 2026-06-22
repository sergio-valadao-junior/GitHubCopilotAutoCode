using GitHubCopilotAutoCode.Data;
using GitHubCopilotAutoCode.Endpoints;
using GitHubCopilotAutoCode.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with InMemory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("GitHubCopilotAutoCodeDb"));

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Map endpoints
app.MapCategoryEndpoints();
app.MapProductEndpoints();

app.Run();
