---
description: "Use when creating or modifying data models and structures in the Models folder. Enforces naming conventions, type safety, sealed classes, DateTime UTC patterns, string initialization, and enum prefixing."
applyTo: "Models/**"
name: "Model Guidelines"
---

# Model Guidelines

These guidelines ensure consistency, type safety, and best practices across all data models in the `Models/` folder.

## General Code Rules

- **GUID ID Property**: Always create an `Id` property of type `GUID` for new models
- **Change Tracking**: Include `CreatedAtUtc` and `UpdatedAtUtc` properties for tracking creation and modification times
- **PascalCase Naming**: All class and property names must be in PascalCase (e.g., `Product`, `CreatedDateUtc`)
- **Type Selection**: Always use the best practices for types, selecting the most appropriate type for each property
- **Single Responsibility**: Every class must be in a separate file named after the class (e.g., `Category.cs` for a `Category` class)
- **Sealed Classes**: Every class in the Models folder must be sealed (`sealed class`) to prevent inheritance unless there is a specific reason to allow it
- **Navigation Properties**: If navigation properties are necessary, they should be at the bottom of the file and marked with a comment indicating they are navigation properties. Use `List<T>` or `IList<T>` for collection navigation properties
- **No Business Logic**: Models should only contain properties and simple validation; any business logic should be implemented in domains, services or other layers of the application

```csharp
public sealed class Product
{
    public Guid Id { get; set; }
    // ... properties
}
```

## DateTime Code Rules

- **UTC Initialization**: All `DateTime` type properties must be initialized using `DateTime.UtcNow`
- **UTC Naming Convention**: Every `DateTime` property must be named with a suffix of `Utc` (e.g., `CreatedDateUtc`, `UpdatedDateUtc`)

```csharp
public sealed class Order
{
    public Guid Id { get; set; }
    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;
}
```

## Text/String Code Rules

- **String Type**: Use `string` type for all text properties
- **Nullable Strings**: Ensure text properties are nullable if they are not required
- **Empty Initialization**: Non-nullable text properties must be initialized with `string.Empty` to avoid null reference issues

```csharp
public sealed class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
```

## List Code Rules

- **Initialization**: Every `List<T>` property must be initialized with `[]` to prevent null reference exceptions
- **Collection Type Selection**: Prefer `List<T>` or `IList<T>` for property declarations to support better abstractions and testability
- **Navigation Properties**: For collection navigation properties (one-to-many relationships), always use `List<T>` or `IList<T>`
- **Immutability**: Use `IReadOnlyList<T>` for read-only collections when the list should not be modified externally
- **PascalCase Naming**: Collection properties must use PascalCase and be named in plural form (e.g., `Items`, `Categories`, `Products`)
- **Type Safety**: Always specify the generic type `T` explicitly; avoid using untyped or dynamic collections

```csharp
public sealed class Order
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public ICollection<OrderItem> Items { get; set; } = [];
    public IReadOnlyList<string> Tags { get; set; } = [];
    public DateTime CreatedDateUtc { get; set; } = DateTime.UtcNow;
}

public sealed class Catalog
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Product> Products { get; set; } = [];
}
```

## Enum Code Rules

- **E Prefix Convention**: Always use the `E` prefix for enum types (e.g., `EPaymentStatus`, `ECategoryType`)
- **PascalCase Values**: Enum values must be in PascalCase without explicit integer values unless necessary for flags or interop scenarios
- **Enums Folder**: Always place enums inside the `./Enums/` folder
- **Single File Per Enum**: Each enum must be in a separate file named after the enum (e.g., `EPaymentStatus.cs` for `EPaymentStatus` enum)

```csharp
// File: Models/Enums/EPaymentStatus.cs
public enum EPaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}
```
