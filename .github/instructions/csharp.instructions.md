---
description: "Use when creating or modifying any C# code (.cs files). Enforces clean code principles, SOLID design patterns, naming conventions, null safety, and best practices across the entire codebase."
applyTo: "**/*.cs"
name: "C# Clean Code Guidelines"
---

# C# Clean Code Guidelines

These guidelines ensure consistency, maintainability, and adherence to SOLID principles across all C# code in the project.

## Code Organization & Formatting

- **Using Statements**: Group `using` statements at the top of each file in this order:
  1. System namespace (`using System;`)
  2. System.\* namespaces (`using System.Collections;`)
  3. Third-party namespaces (`using Stripe;`)
  4. Project namespaces (`using GitHubCopilotAutoCode.Models;`)
  - Leave a blank line between groups
  - Remove unused `using` statements

- **File Structure**: Each class must be in a separate file with the same name as the class (e.g., `Category.cs` contains only the `Category` class)

- **Namespace Declaration**: Use file-scoped namespace declarations for cleaner code:

  ```csharp
  namespace GitHubCopilotAutoCode.Services;

  public sealed class OrderService
  {
  }
  ```

- **Primary Constructors**: Use primary constructors whenever possible (.NET 8+) to reduce boilerplate:

  ```csharp
  // GOOD: Primary constructor
  public sealed class OrderService(IRepository<Order> repository)
  {
      public async Task<Order> GetOrderAsync(Guid id) => await repository.GetByIdAsync(id);
  }

  // AVOID: Traditional constructor
  public sealed class OrderService
  {
      private readonly IRepository<Order> _repository;

      public OrderService(IRepository<Order> repository)
      {
          _repository = repository;
      }
  }
  ```

  - Use primary constructors for dependency injection
  - Primary constructor parameters are automatically available throughout the class
  - Combine with sealed classes and readonly properties for clean, concise code

- **Line Length**: Keep lines under 120 characters for readability

## Naming Conventions

- **Classes & Interfaces**: Use PascalCase (e.g., `OrderService`, `IPaymentProcessor`)
- **Methods & Properties**: Use PascalCase (e.g., `GetProductById()`, `IsActive`)
- **Local Variables & Parameters**: Use camelCase (e.g., `orderCount`, `productName`)
- **Constants**: Use UPPER_SNAKE_CASE for compile-time constants (e.g., `MAX_ITEMS`, `DEFAULT_TIMEOUT`)
- **Private Fields**: Use `_camelCase` prefix (e.g., `_logger`, `_repository`)
- **Interfaces**: Always prefix with `I` (e.g., `IOrderService`, `IRepository<T>`)
- **Asynchronous Methods**: Always suffix with `Async` (e.g., `GetProductAsync()`, `ProcessOrderAsync()`)

## SOLID Principles

### Single Responsibility Principle (SRP)

- Each class should have one reason to change
- Separate concerns: data models in `Models/`, business logic in `Services/`, data access in `Data/`
- Example: A `Product` class should not contain payment logic or HTTP requests

### Open/Closed Principle (OCP)

- Classes should be open for extension but closed for modification
- Use inheritance and interfaces to extend behavior without changing existing code
- Example: Use `IPaymentProcessor` interface to support multiple payment providers

### Liskov Substitution Principle (LSP)

- Derived classes must be substitutable for their base types
- Ensure all implementations of an interface truly fulfill the contract
- Never throw `NotImplementedException` in interface implementations

### Interface Segregation Principle (ISP)

- Create focused, small interfaces rather than large catch-all interfaces
- Example: Prefer `IRepository<T>` over `IDataService` that does everything

### Dependency Inversion Principle (DIP)

- Depend on abstractions, not concrete implementations
- Use constructor injection for dependencies
- Example:

  ```csharp
  public sealed class OrderService
  {
      private readonly IRepository<Order> _repository;

      public OrderService(IRepository<Order> repository)
      {
          _repository = repository;
      }
  }
  ```

## Null Safety & Defensive Programming

- **Null-Check Methods**: Use null-coalescing and null-forgiving operators appropriately:

  ```csharp
  var name = product?.Name ?? "Unknown";
  var count = items?.Count ?? 0;
  ```

- **Argument Validation**: Validate all public method arguments:

  ```csharp
  public void ProcessOrder(Order order)
  {
      if (order == null)
          throw new ArgumentNullException(nameof(order));

      if (string.IsNullOrWhiteSpace(order.OrderNumber))
          throw new ArgumentException("Order number cannot be empty.", nameof(order));
  }
  ```

- **Nullable Reference Types**: Enable nullable reference types in the project; mark all reference types as nullable (`?`) or non-nullable based on actual intent

- **Property Initialization**: Always initialize reference type properties to prevent null reference exceptions:
  ```csharp
  public ICollection<OrderItem> Items { get; set; } = [];
  public string Description { get; set; } = string.Empty;
  ```

## Async/Await Patterns

- **Async All The Way**: Methods that perform I/O or long-running operations must be `async`
- **Naming**: All asynchronous methods must end with `Async` suffix
- **Return Type**: Use `Task` or `Task<T>`, avoid `void` for async methods (except event handlers)
- **No Blocking Calls**: Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` - use `await` instead
- **Example**:
  ```csharp
  public async Task<Order> GetOrderAsync(Guid orderId)
  {
      return await _repository.GetByIdAsync(orderId);
  }
  ```

## Comments & Documentation

- **XML Documentation**: Public classes, methods, and properties must have XML documentation:

  ```csharp
  /// <summary>
  /// Processes a new order and returns the confirmation code.
  /// </summary>
  /// <param name="order">The order to process.</param>
  /// <returns>The unique confirmation code for the order.</returns>
  /// <exception cref="ArgumentNullException">Thrown when order is null.</exception>
  public async Task<string> ProcessOrderAsync(Order order)
  {
  }
  ```

- **Code Comments**: Use sparingly; code should be self-explanatory. Comments should explain _why_, not _what_:

  ```csharp
  // GOOD: Explains the reasoning
  // Retry up to 3 times due to transient network failures
  for (int i = 0; i < 3; i++)

  // BAD: Just repeats the code
  // Loop 3 times
  for (int i = 0; i < 3; i++)
  ```

- **TODO & FIXME**: Use sparingly and only for temporary notes:
  ```csharp
  // TODO: Implement caching strategy after performance baseline is established
  ```

## Error Handling & Validation

- **Exceptions**: Throw specific exception types, not generic `Exception`
- **Custom Exceptions**: Create project-specific exceptions in appropriate namespaces for domain-specific errors
- **Try-Catch**: Use only for exceptional scenarios, not for control flow
- **Validation**: Perform validation early in methods and fail fast:

  ```csharp
  public async Task<Product> GetProductAsync(Guid id)
  {
      if (id == Guid.Empty)
          throw new ArgumentException("Product ID cannot be empty.", nameof(id));

      var product = await _repository.GetByIdAsync(id);
      if (product == null)
          throw new InvalidOperationException($"Product with ID {id} not found.");

      return product;
  }
  ```

## Performance Considerations

- **LINQ Efficiency**: Be aware of when LINQ queries are executed; avoid multiple enumerations:

  ```csharp
  // GOOD: Query executed once
  var activeProducts = _products.Where(p => p.IsActive).ToList();
  var count = activeProducts.Count;

  // BAD: Query executed twice
  var count = _products.Where(p => p.IsActive).Count();
  var list = _products.Where(p => p.IsActive).ToList();
  ```

- **Lazy Loading**: Use `Lazy<T>` for expensive initialization
- **String Operations**: Use `StringBuilder` for multiple string concatenations in loops
- **Collections**: Choose appropriate collection types (`List<T>`, `Dictionary<K,V>`, `HashSet<T>`)

## Type Safety

- **Explicit Types**: Always specify generic types explicitly (`IRepository<Order>` not `IRepository`)
- **Type Inference**: Use `var` only when the type is obvious from context:

  ```csharp
  // GOOD: Type is clear
  var orders = new List<Order>();
  var count = 5;

  // BAD: Type is unclear
  var data = GetData();
  ```

- **Predicates & Delegates**: Use expression-bodied members for simple expressions:
  ```csharp
  public bool IsExpired => DateTime.UtcNow > ExpirationDateUtc;
  public Func<Order, bool> FilterActive => order => order.IsActive;
  ```

## Extension Methods & Utilities

- **Location**: Place all extension methods in a `Extensions/` folder within their appropriate namespace
- **Naming**: Name extension method classes as `[TargetType]Extensions` (e.g., `StringExtensions`, `ListExtensions`)
- **Scope**: Use `public static class` for extension method containers
- **Example**:

  ```csharp
  namespace GitHubCopilotAutoCode.Extensions;

  public static class StringExtensions
  {
      public static bool IsEmpty(this string? value) => string.IsNullOrWhiteSpace(value);
  }
  ```

## Sealed Classes

- **Default to Sealed**: Classes should be sealed by default to prevent unexpected inheritance
- **Allow Inheritance Only When**: There's a documented reason for inheritance patterns
- **Sealed Classes Benefit**: Enables compiler optimizations and prevents misuse

## Constants & Magic Values

- **Avoid Magic Numbers**: Extract magic numbers into named constants:

  ```csharp
  // BAD
  if (items.Count > 100) { }

  // GOOD
  private const int MAX_ITEMS_PER_ORDER = 100;
  if (items.Count > MAX_ITEMS_PER_ORDER) { }
  ```

- **Configuration**: Put application constants in `appsettings.json` or configuration classes

## Model-Specific Rules

For data models in the `Models/` folder, follow the additional rules defined in [Model Guidelines](./models.instructions.md). These include:

- GUID ID properties
- UTC DateTime naming and initialization
- Sealed classes
- String initialization
- List initialization with `[]`
- Enum prefixing with `E`

---

## Summary

Clean code requires:

1. **Clarity**: Code should be easy to understand at a glance
2. **Consistency**: Follow the same patterns throughout the codebase
3. **SOLID Design**: Write flexible, maintainable, and testable code
4. **Simplicity**: Avoid unnecessary complexity; refactor when patterns emerge
5. **Responsibility**: Each component should have a single, well-defined purpose
