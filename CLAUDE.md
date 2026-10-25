# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# Clean Architecture Blazor Server Application Portfolio System

## Project Overview

This is a **Clean Architecture Blazor Server** application built with .NET 9, designed as an enterprise-grade application portfolio and estate planning management system. The project demonstrates modern software architecture principles with AI-assisted development capabilities.

### System Purpose
- **Application Portfolio Management**: Manage IT systems, organizational systems, and capabilities
- **Estate Planning**: Handle estate documents, beneficiaries, and inheritance calculations
- **Enterprise Features**: Multi-tenancy, user management, audit trails, and security analytics

## Technology Stack & Architecture

The solution follows Clean Architecture principles with strict layer separation:

### Core Technologies
- **.NET 9**: Latest .NET framework
- **Blazor Server**: Server-side Blazor for rich UI
- **Entity Framework Core**: ORM with multi-database support (SQL Server, PostgreSQL, SQLite)
- **MediatR**: CQRS pattern implementation
- **MudBlazor**: Material Design component library
- **AutoMapper**: Object-to-object mapping
- **FluentValidation**: Validation framework
- **FusionCache**: Advanced caching layer
- **Hangfire**: Background job processing
- **SignalR**: Real-time communication

### Architecture Layers
```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Server.UI     │    │  Application    │    │     Domain      │
│   (Blazor)      │───▶│   (Business)    │───▶│   (Entities)    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                        │                        
         │              ┌─────────────────┐               
         └─────────────▶│ Infrastructure  │               
                        │   (Data/IO)     │               
                        └─────────────────┘               
```

**Dependency Rules**:
- ✅ Allowed: UI → Application → Domain, Infrastructure → Application → Domain
- ❌ Forbidden: Application → Infrastructure, Domain → Application/Infrastructure/UI

## Essential Commands

### Build and Run
```bash
# Build the solution
dotnet build

# Run the application (Server.UI is the startup project)
dotnet run --project src/Server.UI

# Run tests
dotnet test
```

### Database Management
The project supports multiple database providers configured via `appsettings.json`:

```bash
# SQL Server migrations (primary)
dotnet ef database update --project src/Migrators/Migrators.MSSQL

# PostgreSQL migrations
dotnet ef database update --project src/Migrators/Migrators.PostgreSQL

# SQLite migrations (development)
dotnet ef database update --project src/Migrators/Migrators.SqLite

# Add new migration (example for SQL Server)
dotnet ef migrations add MigrationName --project src/Migrators/Migrators.MSSQL
```

### Configuration
Database provider configuration in `appsettings.json`:
```json
{
  "DatabaseSettings": {
    "DBProvider": "mssql",  // Options: mssql, postgresql, sqlite
    "ConnectionString": "Server=localhost;Database=ApplicationPortfolio;Trusted_Connection=true;"
  }
}
```

## Project Structure & Key Patterns

### CQRS Implementation with DbContextFactory
**CRITICAL**: This project uses `IApplicationDbContextFactory` instead of direct DbContext injection for thread safety and proper disposal:

```csharp
// ✅ CORRECT - Use DbContextFactory in handlers
public class YourHandler : IRequestHandler<YourRequest, Result<YourResponse>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public async Task<Result<YourResponse>> Handle(YourRequest request, CancellationToken ct)
    {
        // Always use 'await using' pattern
        await using var db = await _dbContextFactory.CreateAsync(ct);
        var entity = await db.YourEntities.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        await db.SaveChangesAsync(ct);
        return Result<YourResponse>.Success(response);
    }
}

// ❌ WRONG - Direct DbContext injection (old pattern)
public YourHandler(IApplicationDbContext context) // Don't do this
```

### Feature Organization
Features are organized using CQRS pattern in `Application/Features/{Entity}/`:
```
Features/ITSystems/
├── Commands/
│   ├── Create/CreateITSystemCommand.cs
│   ├── Update/UpdateITSystemCommand.cs
│   └── Delete/DeleteITSystemCommand.cs
├── Queries/
│   ├── GetAll/GetAllITSystemsQuery.cs
│   └── GetById/GetITSystemByIdQuery.cs
├── DTOs/ITSystemDto.cs
└── Security/ITSystemsPermissions.cs
```

### Service Pattern
All business services follow interface-implementation pattern:
- **Interface**: `Application/Common/Interfaces/IYourService.cs`
- **Implementation**: `Infrastructure/Services/YourService.cs`
- **Registration**: `Infrastructure/DependencyInjection.cs`

### Key Domain Entities

1. **ITSystem**: Technology systems tracking (`src/Domain/Entities/ITSystem.cs`)
2. **Capability**: Business capabilities management (`src/Domain/Entities/Capability.cs`)
3. **OrgSystem**: Organizational system management (`src/Domain/Entities/OrgSystem.cs`)
4. **EstateDocument**: Estate planning documents (`src/Domain/Entities/EstateDocument.cs`)
5. **Contact**: Contact management (`src/Domain/Entities/Contact.cs`)

### UI Component Structure
Blazor pages are organized by feature with standard injections:
```razor
@page "/your-feature"
@attribute [Authorize(Policy = Permissions.YourFeature.View)]
@inject IStringLocalizer<YourComponent> L
@inject IMediator Mediator
@inject ISnackbar Snackbar

<PageTitle>@Title</PageTitle>

<MudDataGrid ServerData="ServerReload" T="YourEntityDto">
    <!-- Grid implementation -->
</MudDataGrid>

@code {
    private async Task<GridData<YourEntityDto>> ServerReload(GridState<YourEntityDto> state)
    {
        var query = new GetYourEntitiesQuery();
        var result = await Mediator.Send(query);
        return new GridData<YourEntityDto> { TotalItems = result.TotalItems, Items = result.Items };
    }
}
```

## Security & Permissions

### Authorization System
- **Role-based Access Control**: Admin, Users roles
- **Permission-based Authorization**: Granular permissions per feature
- **Multi-tenancy**: Tenant-level data isolation

### Permission Implementation
```csharp
// Page-level authorization
@attribute [Authorize(Policy = Permissions.Products.View)]

// Component-level permission checks
_accessRights = await PermissionService.GetAccessRightsAsync<ProductsAccessRights>();
if (_accessRights.Create) { /* show create button */ }
```

## Development Patterns & Best Practices

### Adding New Features
1. **Define Domain Entity** in `src/Domain/Entities/`
2. **Create Entity Configuration** in `src/Infrastructure/Persistence/Configurations/`
3. **Add Migration** using Entity Framework
4. **Implement CQRS Pattern** in `src/Application/Features/{Entity}/`
5. **Create UI Components** in `src/Server.UI/Pages/{Entity}/`
6. **Add Navigation** in `MenuService.cs`
7. **Write Tests** in appropriate test projects

### Error Handling Pattern
Use Result pattern throughout:
```csharp
// In handlers
try {
    return Result<T>.Success(data);
} catch (Exception ex) {
    _logger.LogError(ex, "Error in {Handler}", nameof(Handler));
    return Result<T>.Failure("Operation failed");
}

// In UI
var result = await Mediator.Send(command);
if (result.Succeeded) {
    Snackbar.Add("Success!", Severity.Success);
} else {
    Snackbar.Add(result.ErrorMessage, Severity.Error);
}
```

### Performance Optimization
- Use `AsNoTracking()` for read-only queries
- Use `ProjectTo<TDto>()` for efficient mapping
- Implement FusionCache for expensive operations
- Use pagination for large datasets
- Pass `CancellationToken` to all async operations

## Testing Strategy

### Test Projects
- **Domain.UnitTests**: Domain logic testing with xUnit
- **Application.UnitTests**: Application layer testing with FluentAssertions
- **Application.IntegrationTests**: End-to-end testing with Moq

### Test Pattern with DbContextFactory
```csharp
[Test]
public async Task Handle_ValidRequest_ReturnsSuccess()
{
    // Arrange
    var mockFactory = new Mock<IApplicationDbContextFactory>();
    var mockContext = new Mock<IApplicationDbContext>();
    mockFactory.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mockContext.Object);

    var handler = new YourHandler(mockFactory.Object, mapper, logger);
    var request = new YourRequest();

    // Act
    var result = await handler.Handle(request, CancellationToken.None);

    // Assert
    result.Succeeded.Should().BeTrue();
}
```

## AI Development Integration

This project includes comprehensive Cursor AI rules in `.cursor/rules/` that enforce:
- **Clean Architecture principles** (`clean-architecture.md`)
- **Development workflows** (`development-workflows.md`) 
- **CQRS pattern compliance**
- **DbContextFactory usage patterns**
- **Security implementation guides**

The rules provide templates for rapid feature development while maintaining architectural consistency.

## Navigation Structure

The application navigation is configured in `MenuService.cs`:
- **Application Section**: Dashboard, E-Commerce features, Estate Planning
- **Management Section** (Admin): Authorization, System management, Audit trails

## Common Services

### Business Services
- **IInheritanceCalculatorService**: Estate inheritance calculations
- **ISecurityAnalysisService**: Security risk analysis
- **IGeolocationService**: Geographic data processing
- **IPermissionService**: Authorization management

### UI Services  
- **IMenuService**: Navigation management
- **INotificationService**: Real-time notifications

## Docker Support
```bash
# Development with in-memory database
docker run -p 8443:443 -e UseInMemoryDatabase=true blazordevlab/cleanarchitectureblazorserver:latest

# Production deployment via docker-compose
docker-compose up -d
```

Remember: Always use `IApplicationDbContextFactory` with `await using var db = await _dbContextFactory.CreateAsync(ct);` pattern for proper database context management.