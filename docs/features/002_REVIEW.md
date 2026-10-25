# Feature Review: Task 2 — Model core entities (System, Subsystem, Attribute, User, Dependency, AuditLog)

Status: In Progress

## Scope Recap
Implement EF Core entities and baseline persistence for:
- System/SubSystem hierarchy
- Flexible Attribute model
- Dependency edges between systems
- AuditLog support

## What’s Implemented
- Domain entities added:
  - `AttributeDefinition` with `AttributeDataType` enum
  - `SystemAttribute` with unique (OrgSystemId, AttributeDefinitionId)
  - `DependencyEdge` with unique (SourceSystemId, TargetSystemId)
  - Existing: `OrgSystem`, `AuditTrail`, `ITSystem`
- EF Core configurations:
  - `AttributeDefinitionConfiguration`
  - `SystemAttributeConfiguration`
  - `DependencyEdgeConfiguration`
  - Existing: `OrgSystemConfiguration`
- DbContext updates:
  - Added DbSets for AttributeDefinitions, SystemAttributes, DependencyEdges

## Gaps / Next Steps
- Migrations: create initial MSSQL migration covering new entities
- Seeding/indexes: add initial data and performance indexes where relevant
- Unit tests: add tests for configurations and entity constraints
- Optional: Explore navigation collections in `OrgSystem` for `SystemAttributes` and `DependencyEdges` if needed by queries

## Notes on Architecture Compliance
- No DbContext usage in UI; entities/configs live in Domain/Infrastructure as required
- Naming and boundaries adhere to Clean Architecture rules
- Delete behaviors: cascade for attributes, restrict for dependency edges to avoid accidental graph deletion

## Suggested Commands
```
dotnet ef migrations add Init_CoreEntities -p src/Migrators/Migrators.MSSQL -s src/Server.UI
dotnet ef database update -p src/Migrators/Migrators.MSSQL -s src/Server.UI
```


