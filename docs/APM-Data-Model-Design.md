# Application Portfolio Management (APM) Data Model Design

## Overview

This document outlines the design and implementation of core data models for the Application Portfolio Management system, based on the PRD requirements. The implementation follows Clean Architecture patterns and integrates with the existing codebase.

## Core Requirements Met

### 1. System (Core Record)
- **Entity**: `System` - Central catalog with hierarchical structure
- **Features**: Self-referencing parent-child relationships, comprehensive metadata
- **Replaces**: Unifies `ITSystem` and `OrgSystem` into single coherent entity

### 2. Subsystem/Application (Child System)
- **Implementation**: Hierarchical structure via `ParentSystemId` foreign key
- **Features**: Unlimited nesting depth, cascade operations protection

### 3. Attributes (Flexible Metadata Model)
- **Entity**: `SystemAttribute` (enhanced existing)
- **Features**: Dynamic attributes that can be added and backfilled
- **Integration**: Works with existing `AttributeDefinition` system

### 4. Owner/User (Role-based Access)
- **Entity**: `SystemOwnership` - New entity for role assignments
- **Roles**: CIO, Architect, System Owner, Business Owner, Technical Owner, Change Manager, Application Manager, Data Steward
- **Features**: Time-bound ownership, multiple roles per system, audit trail

### 5. Dependency (System-to-System Relationships)
- **Entity**: `SystemDependency` (enhanced from `DependencyEdge`)
- **Types**: API Call, Database, File System, Message Queue, Network, Authentication, Configuration, Third Party, Data Sync, Business Process
- **Features**: Criticality levels, impact assessment, alternative solutions

### 6. Audit Log (Change Tracking)
- **Integration**: Leverages existing `AuditTrail` infrastructure
- **Features**: All changes tracked automatically via BaseAuditableEntity

## Data Model Architecture

### Entity Relationship Diagram
```
System (Core)
├── ParentSystemId (Self-referencing)
├── SystemAttributes (1:Many) → AttributeDefinition
├── SystemOwnerships (1:Many) → ApplicationUser
├── SystemDependencies (1:Many as Source/Target)
└── SystemCapabilities (1:Many) → Capability
```

### Key Entities

#### 1. System Entity
```csharp
public class System : BaseAuditableEntity
{
    // Core Properties
    public string Name { get; set; }
    public SystemType SystemType { get; set; }
    public SystemLifecycleStatus LifecycleStatus { get; set; }
    public SystemCriticality Criticality { get; set; }
    
    // Hierarchical
    public int? ParentSystemId { get; set; }
    public virtual System? ParentSystem { get; set; }
    public virtual ICollection<System> SubSystems { get; set; }
    
    // Relationships
    public virtual ICollection<SystemAttribute> Attributes { get; set; }
    public virtual ICollection<SystemOwnership> Ownerships { get; set; }
    public virtual ICollection<SystemDependency> OutgoingDependencies { get; set; }
    public virtual ICollection<SystemDependency> IncomingDependencies { get; set; }
}
```

#### 2. SystemOwnership Entity
```csharp
public class SystemOwnership : BaseAuditableEntity
{
    public int SystemId { get; set; }
    public string UserId { get; set; }
    public SystemRole Role { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
}
```

#### 3. SystemDependency Entity
```csharp
public class SystemDependency : BaseAuditableEntity
{
    public int SourceSystemId { get; set; }
    public int TargetSystemId { get; set; }
    public DependencyType DependencyType { get; set; }
    public SystemCriticality Criticality { get; set; }
    public string? FailureImpact { get; set; }
    public string? AlternativeSolution { get; set; }
}
```

### Enumerations

#### SystemType
- Application, Infrastructure, Platform, Integration, Data, Security, Monitoring, DevOps

#### SystemRole
- CIO, Architect, SystemOwner, BusinessOwner, TechnicalOwner, ChangeManager, ApplicationManager, DataSteward

#### DependencyType
- ApiCall, Database, FileSystem, MessageQueue, Network, Authentication, Configuration, ThirdParty, DataSync, BusinessProcess

## Database Schema Design

### Key Indexes
- `System.Name` (Unique)
- `System.SystemType`, `System.LifecycleStatus`, `System.Criticality`
- `SystemOwnership.SystemId_UserId_Role`
- `SystemDependency.SourceSystemId_TargetSystemId`

### Constraints
- Self-dependency prevention: `SourceSystemId != TargetSystemId`
- Unique active ownership per role per system
- Cascade delete protection for hierarchical relationships

### Performance Considerations
- Tags stored as pipe-separated strings for efficient querying
- Indexes on frequently filtered columns
- Soft delete support via ISoftDelete interface

## Migration Strategy

### Phase 1: Schema Creation
1. Create new enums: `SystemType`, `SystemRole`, `DependencyType`
2. Create new entities: `System`, `SystemOwnership`, `SystemDependency`
3. Update existing entities: `SystemAttribute`, `SystemCapability`
4. Add Entity Framework configurations
5. Generate and apply migration

### Phase 2: Data Migration (Optional)
1. Migrate existing `ITSystem` data to new `System` table
2. Migrate existing `OrgSystem` data to new `System` table
3. Migrate existing `DependencyEdge` data to new `SystemDependency` table
4. Update references in `SystemAttribute` and `SystemCapability`

### Phase 3: Application Layer Updates
1. Update CQRS handlers to use new entities
2. Create new DTOs for new entities
3. Update UI components
4. Add new permission definitions

### Phase 4: Legacy Cleanup (Future)
1. Deprecate old entities (`ITSystem`, `OrgSystem`, `DependencyEdge`)
2. Remove unused configurations and migrations
3. Update documentation

## Implementation Files Created

### Domain Layer
- `C:\Development\GitHub\ApplicationPortfolio\src\Domain\Enums\SystemType.cs`
- `C:\Development\GitHub\ApplicationPortfolio\src\Domain\Enums\SystemRole.cs`
- `C:\Development\GitHub\ApplicationPortfolio\src\Domain\Enums\DependencyType.cs`
- `C:\Development\GitHub\ApplicationPortfolio\src\Domain\Entities\System.cs`
- `C:\Development\GitHub\ApplicationPortfolio\src\Domain\Entities\SystemOwnership.cs`
- `C:\Development\GitHub\ApplicationPortfolio\src\Domain\Entities\SystemDependency.cs`

### Infrastructure Layer
- `C:\Development\GitHub\ApplicationPortfolio\src\Infrastructure\Persistence\Configurations\SystemConfiguration.cs`
- `C:\Development\GitHub\ApplicationPortfolio\src\Infrastructure\Persistence\Configurations\SystemOwnershipConfiguration.cs`
- `C:\Development\GitHub\ApplicationPortfolio\src\Infrastructure\Persistence\Configurations\SystemDependencyConfiguration.cs`

### Updated Files
- Updated `SystemAttribute.cs` to reference new `System` entity
- Updated `SystemCapability.cs` to reference new `System` entity
- Updated `SystemAttributeConfiguration.cs` for new relationships
- Updated `SystemCapabilityConfiguration.cs` for new relationships
- Updated `IApplicationDbContext.cs` with new DbSets
- Updated `ApplicationDbContext.cs` with new DbSets

## Next Steps

1. **Generate Migration**:
   ```bash
   dotnet ef migrations add "Add_APM_System_Entities" --project src/Migrators/Migrators.MSSQL
   ```

2. **Apply Migration**:
   ```bash
   dotnet ef database update --project src/Migrators/Migrators.MSSQL
   ```

3. **Implement CQRS Features**:
   - Create `Application/Features/Systems/` folder structure
   - Implement Commands (Create, Update, Delete)
   - Implement Queries (GetAll, GetById, GetHierarchy)
   - Add DTOs and AutoMapper profiles

4. **Create UI Components**:
   - System management pages
   - Hierarchical system browser
   - Dependency visualization
   - Ownership management interface

5. **Add Security**:
   - Define permissions for system management
   - Implement role-based access control
   - Add authorization policies

## Benefits Achieved

1. **Unified Data Model**: Single `System` entity replaces fragmented `ITSystem`/`OrgSystem`
2. **Flexible Metadata**: Dynamic attributes support evolving requirements
3. **Role-based Access**: Comprehensive ownership and role management
4. **Rich Dependencies**: Detailed relationship modeling with impact assessment
5. **Audit Compliance**: Full change tracking and accountability
6. **Scalable Architecture**: Hierarchical structure supports any organizational complexity
7. **Performance Optimized**: Proper indexing and efficient storage patterns

The implementation provides a solid foundation for enterprise-grade Application Portfolio Management while maintaining compatibility with existing Clean Architecture patterns.