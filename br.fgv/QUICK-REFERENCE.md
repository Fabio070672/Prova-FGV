# Clean Architecture Quick Reference

## Layer Dependency Rules

```
API ? Application + Infrastructure
Infrastructure ? Domain + Application
Application ? Domain
Domain ? SharedKernel
```

## Project Structure

```
src/
??? FGV.Api/                    # REST API Layer
?   ??? Controllers/            # API Controllers
?   ??? Program.cs             # App entry point
?   ??? appsettings.json       # Configuration
?
??? FGV.Application/           # Application Layer (CQRS)
?   ??? Abstractions/
?   ?   ??? Messaging/         # Command/Query interfaces
?   ?   ??? Authentication/    # User context
?   ??? Common/
?   ?   ??? Pagination/        # Paging support
?   ??? {Entity}s/
?   ?   ??? Create/           # Create command + handler
?   ?   ??? Update/           # Update command + handler
?   ?   ??? Delete/           # Delete command + handler
?   ?   ??? GetById/          # Query + handler
?   ?   ??? GetAll/           # Query + handler (paged)
?   ?   ??? {Entity}Response.cs
?   ??? DependencyInjection.cs
?
??? FGV.Domain/                # Domain Layer
?   ??? {Entity}s/
?       ??? {Entity}.cs        # Entity (factory pattern)
?       ??? {Entity}Id.cs      # Strongly-typed ID
?       ??? I{Entity}Repository.cs  # Repository interface
?       ??? {Entity}Errors.cs  # Domain errors
?
??? FGV.Infrastructure/        # Infrastructure Layer
?   ??? Configurations/
?   ?   ??? {Entity}s/
?   ?       ??? {Entity}Configuration.cs  # EF config
?   ??? Repositories/
?   ?   ??? Repository.cs      # Base repository
?   ?   ??? {Entity}Repository.cs
?   ??? ApplicationDbContext.cs
?   ??? UnitOfWork.cs
?   ??? DependencyInjection.cs
?
??? FGV.SharedKernel/          # Shared Kernel
    ??? Entity.cs              # Base entity
    ??? Result.cs              # Result pattern
    ??? Error.cs               # Error record
    ??? IUnitOfWork.cs         # UoW interface
```

## Creating a New Entity - Checklist

### 1. Domain Layer (`FGV.Domain/{Entity}s/`)
- [ ] `{Entity}Id.cs` - Strongly-typed ID record
- [ ] `{Entity}.cs` - Entity with private constructor and factory method
- [ ] `I{Entity}Repository.cs` - Repository interface
- [ ] `{Entity}Errors.cs` - Domain errors static class

### 2. Infrastructure Layer
- [ ] `Repositories/{Entity}Repository.cs` - Implement repository
- [ ] `Configurations/{Entity}s/{Entity}Configuration.cs` - EF configuration
- [ ] Add `DbSet<{Entity}>` to `ApplicationDbContext`
- [ ] Register repository in `DependencyInjection.cs`

### 3. Application Layer (`FGV.Application/{Entity}s/`)
- [ ] `{Entity}Response.cs` - Response DTO
- [ ] `Create/Create{Entity}Command.cs`
- [ ] `Create/Create{Entity}Handler.cs`
- [ ] `Create/Create{Entity}Validator.cs`
- [ ] Repeat for Update, Delete, GetById, GetAll
- [ ] Register handlers in `DependencyInjection.cs`

### 4. API Layer
- [ ] `Controllers/{Entity}s/{Entity}sController.cs`
- [ ] Add request DTOs if needed

### 5. Database
- [ ] Create migration: `dotnet ef migrations add Add{Entity}`
- [ ] Update database: `dotnet ef database update`

## Code Templates

### Strongly-Typed ID
```csharp
public record ProductId(string Value)
{
    public static ProductId NewId() => new(Guid.NewGuid().ToString());
}
```

### Entity
```csharp
public sealed class Product : Entity<ProductId>
{
    private Product() { }
    
    private Product(ProductId id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
        CreatedAt = DateTime.UtcNow;
        Active = true;
    }

    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }

    public static Product Create(string name, decimal price)
    {
        return new Product(ProductId.NewId(), name, price);
    }

    public void Update(string name, decimal price)
    {
        Name = name;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### Repository Interface
```csharp
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct = default);
    void Add(Product product);
    void Remove(Product product);
}
```

### Repository Implementation
```csharp
internal sealed class ProductRepository 
    : Repository<Product, ProductId>, IProductRepository
{
    public ProductRepository(ApplicationDbContext dbContext) 
        : base(dbContext) { }
}
```

### EF Configuration
```csharp
internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasConversion(id => id.Value, value => new ProductId(value))
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Price)
            .HasColumnName("price")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
```

### Command
```csharp
public sealed record CreateProductCommand(
    string Name,
    decimal Price) : ICommand<string>;
```

### Handler
```csharp
internal sealed class CreateProductHandler 
    : ICommandHandler<CreateProductCommand, string>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductHandler(
        IProductRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> HandleAsync(
        CreateProductCommand command,
        CancellationToken cancellationToken)
    {
        var product = Product.Create(command.Name, command.Price);

        _repository.Add(product);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Id.Value);
    }
}
```

### Controller
```csharp
[Route("api/products")]
[ApiController]
public sealed class ProductsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateProductRequest request,
        ICommandHandler<CreateProductCommand, string> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request.Name, request.Price);

        Result<string> result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            result.Value);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        string id,
        IQueryHandler<GetProductByIdQuery, ProductResponse> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery(id);

        Result<ProductResponse> result = await handler.HandleAsync(query, cancellationToken);

        return result.IsFailure ? NotFound(result.Error) : Ok(result.Value);
    }
}
```

## Common EF Core Commands

```bash
# Create migration
cd src/FGV.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../FGV.Api

# Apply migrations
dotnet ef database update --startup-project ../FGV.Api

# Remove last migration (if not applied)
dotnet ef migrations remove --startup-project ../FGV.Api

# Generate SQL script
dotnet ef migrations script --startup-project ../FGV.Api

# Drop database
dotnet ef database drop --startup-project ../FGV.Api
```

## Naming Conventions

### C# (PascalCase)
- Classes: `Product`, `ProductRepository`
- Methods: `GetByIdAsync`, `Create`
- Properties: `Name`, `CreatedAt`

### Database (snake_case)
- Tables: `products`, `product_categories`
- Columns: `id`, `product_name`, `created_at`

### Files
- Commands: `CreateProductCommand.cs`
- Handlers: `CreateProductHandler.cs`
- Validators: `CreateProductValidator.cs`
- Controllers: `ProductsController.cs`

## Key Principles

1. **Dependency Rule**: Dependencies point inward (toward Domain)
2. **Repository Interfaces**: Always in Domain layer
3. **Repository Implementations**: Always in Infrastructure layer
4. **No Business Logic**: In controllers or infrastructure
5. **Factory Pattern**: For entity creation
6. **Result Pattern**: For error handling
7. **CQRS**: Separate read and write operations
8. **Strongly-Typed IDs**: For all entities

## HTTP Status Codes

- `200 OK` - Successful GET
- `201 Created` - Successful POST
- `204 No Content` - Successful PUT/DELETE
- `400 Bad Request` - Validation error
- `404 Not Found` - Resource not found
- `500 Internal Server Error` - Unexpected error
