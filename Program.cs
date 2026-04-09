using GitHubCopilotAutoCode.Data;
using GitHubCopilotAutoCode.Endpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with InMemory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("GitHubCopilotAutoCodeDb"));

var app = builder.Build();

// Map endpoints
app.MapCategoryEndpoints();
app.MapProductEndpoints();

app.Run();
