# FGV Clean Architecture Project

This project follows Clean Architecture principles with CQRS pattern.

## Architecture Overview

```
???????????????????????????????????
?         FGV.Api                 ?
?  (Controllers, Endpoints)       ?
???????????????????????????????????
           ? depends on
    ???????????????????????
    ?                     ?
???????????????  ???????????????????
? Application ?  ? Infrastructure  ?
? (CQRS)      ?  ? (EF, Repos)     ?
???????????????  ???????????????????
    ? depends on     ? implements
    ??????????????????
          ?
    ?????????????
    ?  Domain   ?
    ? (Entities)?
    ?????????????
          ? depends on
    ???????????????
    ? SharedKernel?
    ? (Base Types)?
    ???????????????
```

## Project Structure

### 1. FGV.SharedKernel
Base classes and common interfaces used across all layers.

**Key Files:**
- `Entity.cs` - Base entity class with Id, timestamps, and audit fields
- `Result.cs` - Result pattern for error handling
- `Error.cs` - Error record for standardized errors
- `IUnitOfWork.cs` - Unit of Work pattern interface

### 2. FGV.Domain
Core business logic and domain entities.

**Structure:**
```
FGV.Domain/
??? {EntityName}s/
?   ??? {EntityName}.cs          # Domain entity
?   ??? {EntityName}Id.cs        # Strongly-typed ID
?   ??? I{EntityName}Repository.cs # Repository interface
?   ??? {EntityName}Errors.cs    # Domain errors
```

**Rules:**
- ? Entities with private constructors
- ? Factory methods for creation
- ? Strongly-typed IDs
- ? Repository interfaces (NOT implementations)
- ? NO dependencies on Application or Infrastructure

### 3. FGV.Application
Application business logic using CQRS pattern.

**Structure:**
```
FGV.Application/
??? Abstractions/
?   ??? Messaging/
?   ?   ??? ICommand.cs
?   ?   ??? ICommandHandler.cs
?   ?   ??? IQuery.cs
?   ?   ??? IQueryHandler.cs
?   ??? Authentication/
?       ??? IUserContext.cs
??? Common/
?   ??? Pagination/
?       ??? PagedResult.cs
??? {EntityName}s/
?   ??? Create/
?   ?   ??? Create{EntityName}Command.cs
?   ?   ??? Create{EntityName}Handler.cs
?   ?   ??? Create{EntityName}Validator.cs
?   ??? Update/
?   ??? Delete/
?   ??? GetById/
?   ??? GetAll/
?   ??? {EntityName}Response.cs
??? DependencyInjection.cs
```

**Rules:**
- ? CQRS pattern (Commands and Queries)
- ? Each command/query has its own handler
- ? FluentValidation for validation
- ? DTOs for responses
- ? NO dependencies on Infrastructure

### 4. FGV.Infrastructure
Infrastructure concerns (database, external services).

**Structure:**
```
FGV.Infrastructure/
??? Configurations/
?   ??? {EntityName}s/
?       ??? {EntityName}Configuration.cs  # EF Core configuration
??? Repositories/
?   ??? Repository.cs                     # Base repository
?   ??? {EntityName}Repository.cs         # Entity repository
??? Data/
?   ??? Migrations/
??? ApplicationDbContext.cs
??? UnitOfWork.cs
??? DependencyInjection.cs
```

**Rules:**
- ? Implements Domain repository interfaces
- ? EF Core configurations using Fluent API
- ? Snake_case naming for database objects
- ? Registers services in DependencyInjection

### 5. FGV.Api
RESTful API endpoints and HTTP concerns.

**Structure:**
```
FGV.Api/
??? Controllers/
?   ??? {EntityName}s/
?       ??? {EntityName}sController.cs
??? Program.cs
??? appsettings.json
```

**Rules:**
- ? Thin controllers (just routing)
- ? Dependency injection for handlers
- ? Proper HTTP status codes
- ? Request/Response DTOs
- ? NO business logic in controllers

## Naming Conventions

### Database (PostgreSQL)
- **Tables**: `snake_case` (e.g., `medical_specialties`)
- **Columns**: `snake_case` (e.g., `created_at`, `updated_by`)
- **Primary Keys**: `id`

### C# Code
- **Classes**: `PascalCase` (e.g., `MedicalSpecialty`)
- **Properties**: `PascalCase` (e.g., `CreatedAt`)
- **Methods**: `PascalCase` (e.g., `GetByIdAsync`)
- **Parameters**: `camelCase` (e.g., `specialtyId`)

## Dependency Injection Registration

### In FGV.Application/DependencyInjection.cs
```csharp
services.AddScoped<ICommandHandler<CreateEntityCommand, string>, CreateEntityHandler>();
services.AddScoped<IQueryHandler<GetEntityByIdQuery, EntityResponse>, GetEntityByIdHandler>();
```

### In FGV.Infrastructure/DependencyInjection.cs
```csharp
services.AddScoped<IEntityRepository, EntityRepository>();
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
```

## Running the Application

1. **Update Connection String** in `appsettings.json`
2. **Create Database Migration**:
   ```bash
   cd src/FGV.Infrastructure
   dotnet ef migrations add InitialCreate --startup-project ../FGV.Api
   ```
3. **Update Database**:
   ```bash
   dotnet ef database update --startup-project ../FGV.Api
   ```
4. **Run API**:
   ```bash
   cd src/FGV.Api
   dotnet run
   ```

## Creating a New Entity (Quick Guide)

1. **Domain Layer**:
   - Create `{EntityName}Id.cs`
   - Create `{EntityName}.cs` with factory method
   - Create `I{EntityName}Repository.cs`
   - Create `{EntityName}Errors.cs`

2. **Infrastructure Layer**:
   - Create `{EntityName}Repository.cs`
   - Create `{EntityName}Configuration.cs`
   - Register repository in `DependencyInjection.cs`
   - Add `DbSet<{EntityName}>` to `ApplicationDbContext`

3. **Application Layer**:
   - Create `{EntityName}Response.cs`
   - Create CQRS commands/queries with handlers
   - Register handlers in `DependencyInjection.cs`

4. **API Layer**:
   - Create `{EntityName}sController.cs`
   - Add endpoints mapping to handlers

## Tools & Libraries

- **Entity Framework Core** - ORM
- **Npgsql** - PostgreSQL provider
- **FluentValidation** - Input validation
- **Swagger** - API documentation

## Best Practices

? **DO**:
- Use Result pattern for error handling
- Keep controllers thin
- Use strongly-typed IDs
- Follow CQRS pattern strictly
- Use async/await everywhere
- Pass CancellationToken through layers

? **DON'T**:
- Put business logic in controllers
- Use concrete implementations in Application layer
- Create entities with public constructors
- Mix commands and queries
- Return void from handlers
- Put repository interfaces in Application layer
