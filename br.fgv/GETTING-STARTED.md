# FGV Clean Architecture - Getting Started

## Project Structure Created

The following Clean Architecture layers have been successfully created:

### ? Projects Created

1. **FGV.SharedKernel** - Base classes and common interfaces
   - `Entity.cs` - Base entity with Id and audit fields
   - `Result.cs` - Result pattern for error handling
   - `Error.cs` - Standardized error records
   - `IUnitOfWork.cs` - Unit of Work pattern

2. **FGV.Domain** - Domain entities and business rules
   - Ready for entity creation
   - Repository interfaces belong here

3. **FGV.Application** - Application logic with CQRS
   - `Abstractions/Messaging/` - Command/Query handlers interfaces
   - `Abstractions/Authentication/` - User context interface
   - `Common/Pagination/` - Pagination support
   - `DependencyInjection.cs` - Service registration

4. **FGV.Infrastructure** - Data access and external services
   - `ApplicationDbContext.cs` - EF Core DbContext
   - `UnitOfWork.cs` - Unit of Work implementation
   - `Repositories/Repository.cs` - Base repository
   - `DependencyInjection.cs` - Infrastructure services registration
   - Entity Framework Core packages installed
   - PostgreSQL provider configured

5. **FGV.Api** - REST API endpoints
   - `Program.cs` - Configured with all layers
   - `Controllers/` - Ready for controllers
   - Swagger/OpenAPI configured
   - Connection string configured

### ?? NuGet Packages Installed

**FGV.Infrastructure:**
- Microsoft.EntityFrameworkCore (10.0.5)
- Npgsql.EntityFrameworkCore.PostgreSQL (10.0.1)
- Microsoft.EntityFrameworkCore.Design (10.0.5)

**FGV.Application:**
- Microsoft.Extensions.DependencyInjection.Abstractions (10.0.5)

**FGV.Api:**
- Swashbuckle.AspNetCore (10.1.7)

### ?? Project References

```
FGV.Api
  ??> FGV.Application
  ??> FGV.Infrastructure

FGV.Infrastructure
  ??> FGV.Domain
  ??> FGV.Application

FGV.Application
  ??> FGV.Domain

FGV.Domain
  ??> FGV.SharedKernel
```

## Next Steps

### 1. Configure Database Connection

Edit `src/FGV.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=fgv_db;Username=your_user;Password=your_password"
  }
}
```

### 2. Create Your First Entity

Follow the Clean Architecture Agent templates to create a new entity. For example, to create a "Product" entity:

#### Step 1: Domain Layer

Create in `src/FGV.Domain/Products/`:

1. `ProductId.cs` - Strongly-typed ID
```csharp
namespace FGV.Domain.Products;

public record ProductId(string Value)
{
    public static ProductId NewId() => new(Guid.NewGuid().ToString());
}
```

2. `Product.cs` - Entity with factory method
3. `IProductRepository.cs` - Repository interface
4. `ProductErrors.cs` - Domain errors

#### Step 2: Infrastructure Layer

Create in `src/FGV.Infrastructure/`:

1. `Repositories/ProductRepository.cs` - Repository implementation
2. `Configurations/Products/ProductConfiguration.cs` - EF configuration

Register in `DependencyInjection.cs`:
```csharp
services.AddScoped<IProductRepository, ProductRepository>();
```

Add to `ApplicationDbContext.cs`:
```csharp
public DbSet<Product> Products => Set<Product>();
```

#### Step 3: Application Layer

Create in `src/FGV.Application/Products/`:

1. `ProductResponse.cs` - DTO
2. `Create/CreateProductCommand.cs` - Command
3. `Create/CreateProductHandler.cs` - Handler
4. `Create/CreateProductValidator.cs` - Validator

Register in `DependencyInjection.cs`:
```csharp
services.AddScoped<ICommandHandler<CreateProductCommand, string>, CreateProductHandler>();
```

#### Step 4: API Layer

Create `src/FGV.Api/Controllers/Products/ProductsController.cs`

### 3. Create Database Migration

```bash
cd src/FGV.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../FGV.Api
dotnet ef database update --startup-project ../FGV.Api
```

### 4. Run the Application

```bash
cd src/FGV.Api
dotnet run
```

Access Swagger UI at: `https://localhost:5001/swagger`

## Clean Architecture Principles

### ? DO:
- Put repository **interfaces** in Domain layer
- Put repository **implementations** in Infrastructure layer
- Use factory methods for entity creation
- Return `Result<T>` from all handlers
- Keep controllers thin (just routing)
- Use strongly-typed IDs
- Follow CQRS pattern (separate Commands and Queries)

### ? DON'T:
- Put business logic in controllers
- Create entities with public constructors
- Put repository interfaces in Application layer
- Use MediatR directly (use ICommandHandler/IQueryHandler)
- Mix commands and queries
- Return void from handlers

## File Naming Conventions

- **Commands**: `{Action}{Entity}Command.cs` (e.g., `CreateProductCommand.cs`)
- **Handlers**: `{Action}{Entity}Handler.cs` (e.g., `CreateProductHandler.cs`)
- **Validators**: `{Action}{Entity}Validator.cs` (e.g., `CreateProductValidator.cs`)
- **Queries**: `{Action}{Entity}Query.cs` (e.g., `GetProductByIdQuery.cs`)
- **DTOs**: `{Entity}Response.cs` (e.g., `ProductResponse.cs`)
- **Controllers**: `{Entity}sController.cs` (e.g., `ProductsController.cs`)

## Database Naming Conventions

- **Tables**: `snake_case` plural (e.g., `products`)
- **Columns**: `snake_case` (e.g., `created_at`, `product_name`)
- **Primary Keys**: `id`
- **Foreign Keys**: `{table}_id` (e.g., `category_id`)

## Common Commands

### Build Solution
```bash
dotnet build
```

### Run Tests (when created)
```bash
dotnet test
```

### Create Migration
```bash
cd src/FGV.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../FGV.Api
```

### Update Database
```bash
dotnet ef database update --startup-project ../FGV.Api
```

### Remove Last Migration
```bash
dotnet ef migrations remove --startup-project ../FGV.Api
```

## Additional Resources

Refer to the Clean Architecture Agent documentation for detailed templates and examples:
- `.github/copilot/copilot-agents/clean-architecture-agent.md`
- `README.md` - Complete architecture overview

## Support

For issues or questions about the Clean Architecture implementation, refer to the templates in the Clean Architecture Agent documentation.
