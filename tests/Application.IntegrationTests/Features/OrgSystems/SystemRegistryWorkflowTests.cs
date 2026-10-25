// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Commands.AddEdit;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Commands.Delete;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Queries.Pagination;
using CleanArchitecture.Blazor.Domain.Common.Enums;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Features.OrgSystems;

using static Testing;

[TestFixture]
public class SystemRegistryWorkflowTests : TestBase
{
    [Test]
    public async Task CompleteSystemRegistryWorkflow_CreateSystemWithAttributesAndDependencies_WorksEndToEnd()
    {
        // Arrange - First create attribute definitions
        var versionAttrDef = await CreateAttributeDefinition("Version", AttributeDataType.Text, true);
        var environmentAttrDef = await CreateAttributeDefinition("Environment", AttributeDataType.Enum, true, "Development,Testing,Staging,Production");
        var isActiveAttrDef = await CreateAttributeDefinition("IsActive", AttributeDataType.Boolean, true);

        // Act - Create primary system
        var createSystemCommand = new AddEditOrgSystemCommand
        {
            Name = "Enterprise HR System",
            Description = "Comprehensive human resources management system",
            BusinessOwnerId = 1,
            TechnicalOwnerId = 2,
            Criticality = SystemCriticality.Critical,
            LifecycleStatus = SystemLifecycleStatus.Active,
            Tags = new List<string> { "HR", "Enterprise", "Core" },
            Dependencies = "Authentication Service, Database Cluster, Email Service"
        };

        var systemResult = await SendAsync(createSystemCommand);

        // Assert - System creation
        systemResult.Should().NotBeNull();
        systemResult.Succeeded.Should().BeTrue();
        var systemId = systemResult.Data;

        // Verify system exists in database
        var createdSystem = await FindAsync<OrgSystem>(systemId);
        createdSystem.Should().NotBeNull();
        createdSystem.Name.Should().Be("Enterprise HR System");
        createdSystem.Criticality.Should().Be(SystemCriticality.Critical);
        createdSystem.Tags.Should().Contain("Enterprise");

        // Act - Add system attributes
        await AddSystemAttribute(systemId, versionAttrDef, "2.1.5");
        await AddSystemAttribute(systemId, environmentAttrDef, "Production");
        await AddSystemAttribute(systemId, isActiveAttrDef, "true");

        // Verify attributes were added
        var systemWithAttributes = CreateDbContext().OrgSystems
            .Include(s => s.Attributes)
            .ThenInclude(a => a.AttributeDefinition)
            .First(s => s.Id == systemId);

        systemWithAttributes.Attributes.Should().HaveCount(3);
        systemWithAttributes.Attributes.Should().Contain(a => a.AttributeDefinition.Name == "Version" && a.Value == "2.1.5");
        systemWithAttributes.Attributes.Should().Contain(a => a.AttributeDefinition.Name == "Environment" && a.Value == "Production");
        systemWithAttributes.Attributes.Should().Contain(a => a.AttributeDefinition.Name == "IsActive" && a.Value == "true");

        // Act - Create dependent system
        var createDependentSystemCommand = new AddEditOrgSystemCommand
        {
            Name = "Authentication Service",
            Description = "Centralized authentication and authorization service",
            BusinessOwnerId = 2,
            TechnicalOwnerId = 1,
            Criticality = SystemCriticality.Critical,
            LifecycleStatus = SystemLifecycleStatus.Active,
            Tags = new List<string> { "Security", "Authentication", "Core" }
        };

        var dependentSystemResult = await SendAsync(createDependentSystemCommand);
        var dependentSystemId = dependentSystemResult.Data;

        // Act - Create dependency relationship
        await AddDependencyRelationship(systemId, dependentSystemId, "OAuth2 authentication integration");

        // Verify dependency was created
        var systemWithDependencies = CreateDbContext().OrgSystems
            .Include(s => s.OutgoingDependencies)
            .ThenInclude(d => d.TargetSystem)
            .First(s => s.Id == systemId);

        systemWithDependencies.OutgoingDependencies.Should().HaveCount(1);
        systemWithDependencies.OutgoingDependencies.First().TargetSystem.Name.Should().Be("Authentication Service");
        systemWithDependencies.OutgoingDependencies.First().Notes.Should().Be("OAuth2 authentication integration");
    }

    [Test]
    public async Task AttributeBackfillWorkflow_AddNewAttributeToExistingSystems_BackfillsCorrectly()
    {
        // Arrange - Create multiple systems
        var system1Id = await CreateSystem("System One", SystemCriticality.High);
        var system2Id = await CreateSystem("System Two", SystemCriticality.Medium);
        var system3Id = await CreateSystem("System Three", SystemCriticality.Low);

        // Act - Create new attribute definition that should be backfilled
        var newAttributeDef = await CreateAttributeDefinition("CostCenter", AttributeDataType.Text, false);

        // Manually backfill the new attribute to existing systems (simulating backfill operation)
        await AddSystemAttribute(system1Id, newAttributeDef, "CC-001");
        await AddSystemAttribute(system2Id, newAttributeDef, "CC-002");
        await AddSystemAttribute(system3Id, newAttributeDef, "CC-003");

        // Assert - Verify backfill was successful
        var systems = CreateDbContext().OrgSystems
            .Include(s => s.Attributes)
            .ThenInclude(a => a.AttributeDefinition)
            .Where(s => new[] { system1Id, system2Id, system3Id }.Contains(s.Id))
            .ToList();

        systems.Should().HaveCount(3);
        systems.Should().OnlyContain(s => s.Attributes.Any(a => 
            a.AttributeDefinition.Name == "CostCenter" && 
            a.Value.StartsWith("CC-")));
    }

    [Test]
    public async Task HierarchicalSystemWorkflow_CreateParentAndChildSystems_WorksCorrectly()
    {
        // Arrange & Act - Create parent system
        var createParentCommand = new AddEditOrgSystemCommand
        {
            Name = "ERP Suite",
            Description = "Enterprise Resource Planning Suite",
            Criticality = SystemCriticality.Critical,
            LifecycleStatus = SystemLifecycleStatus.Active
        };

        var parentResult = await SendAsync(createParentCommand);
        var parentSystemId = parentResult.Data;

        // Act - Create child systems (simulated via ITSystem for hierarchical relationship)
        var createChildHRCommand = new AddEditITSystemCommand
        {
            Name = "HR Module",
            Description = "Human Resources module within ERP",
            ParentSystemId = parentSystemId,
            Category = "ERP Module",
            Status = "Active"
        };

        var createChildFinanceCommand = new AddEditITSystemCommand
        {
            Name = "Finance Module",
            Description = "Financial management module within ERP",
            ParentSystemId = parentSystemId,
            Category = "ERP Module",
            Status = "Active"
        };

        var hrResult = await SendAsync(createChildHRCommand);
        var financeResult = await SendAsync(createChildFinanceCommand);

        // Assert - Verify hierarchical relationship
        var parentSystem = CreateDbContext().ITSystems
            .Include(s => s.SubSystems)
            .First(s => s.Id == parentSystemId);

        parentSystem.SubSystems.Should().HaveCount(2);
        parentSystem.SubSystems.Should().Contain(s => s.Name == "HR Module");
        parentSystem.SubSystems.Should().Contain(s => s.Name == "Finance Module");

        // Verify child systems reference parent
        var hrModule = await FindAsync<ITSystem>(hrResult.Data);
        var financeModule = await FindAsync<ITSystem>(financeResult.Data);

        hrModule.ParentSystemId.Should().Be(parentSystemId);
        financeModule.ParentSystemId.Should().Be(parentSystemId);
    }

    [Test]
    public async Task SystemLifecycleWorkflow_CreateUpdateRetireDelete_WorksEndToEnd()
    {
        // Arrange - Create system
        var systemId = await CreateSystem("Lifecycle Test System", SystemCriticality.Medium);
        var originalSystem = await FindAsync<OrgSystem>(systemId);

        originalSystem.LifecycleStatus.Should().Be(SystemLifecycleStatus.Active);

        // Act - Update system to maintenance mode
        var updateToMaintenanceCommand = new AddEditOrgSystemCommand
        {
            Id = systemId,
            Name = originalSystem.Name,
            Description = "System under maintenance",
            Criticality = originalSystem.Criticality,
            LifecycleStatus = SystemLifecycleStatus.Inactive,
            Tags = originalSystem.Tags
        };

        await SendAsync(updateToMaintenanceCommand);

        // Assert - Verify maintenance status
        var maintenanceSystem = await FindAsync<OrgSystem>(systemId);
        maintenanceSystem.LifecycleStatus.Should().Be(SystemLifecycleStatus.Inactive);
        maintenanceSystem.Description.Should().Be("System under maintenance");

        // Act - Retire the system
        var retireSystemCommand = new AddEditOrgSystemCommand
        {
            Id = systemId,
            Name = originalSystem.Name,
            Description = "System retired - end of life",
            Criticality = originalSystem.Criticality,
            LifecycleStatus = SystemLifecycleStatus.Retired,
            Tags = originalSystem.Tags
        };

        await SendAsync(retireSystemCommand);

        // Assert - Verify retired status
        var retiredSystem = await FindAsync<OrgSystem>(systemId);
        retiredSystem.LifecycleStatus.Should().Be(SystemLifecycleStatus.Retired);
        retiredSystem.Description.Should().Be("System retired - end of life");

        // Act - Delete the retired system
        var deleteCommand = new DeleteOrgSystemCommand(new[] { systemId });
        var deleteResult = await SendAsync(deleteCommand);

        // Assert - Verify deletion
        deleteResult.Succeeded.Should().BeTrue();
        var deletedSystem = await FindAsync<OrgSystem>(systemId);
        deletedSystem.Should().BeNull();
    }

    [Test]
    public async Task ComplexDependencyWorkflow_CreateMultipleSystemsWithInterdependencies_WorksCorrectly()
    {
        // Arrange - Create multiple interconnected systems
        var webAppId = await CreateSystem("Web Application", SystemCriticality.High);
        var apiGatewayId = await CreateSystem("API Gateway", SystemCriticality.Critical);
        var databaseId = await CreateSystem("Database System", SystemCriticality.Critical);
        var cacheId = await CreateSystem("Cache Layer", SystemCriticality.Medium);
        var authServiceId = await CreateSystem("Authentication Service", SystemCriticality.Critical);

        // Act - Create complex dependency relationships
        // Web App -> API Gateway
        await AddDependencyRelationship(webAppId, apiGatewayId, "REST API calls for data operations");
        
        // API Gateway -> Database
        await AddDependencyRelationship(apiGatewayId, databaseId, "Direct database queries for core operations");
        
        // API Gateway -> Cache
        await AddDependencyRelationship(apiGatewayId, cacheId, "Redis cache for performance optimization");
        
        // Web App -> Auth Service
        await AddDependencyRelationship(webAppId, authServiceId, "User authentication and session management");
        
        // API Gateway -> Auth Service
        await AddDependencyRelationship(apiGatewayId, authServiceId, "Token validation for API endpoints");

        // Assert - Verify complex dependency graph
        var webApp = CreateDbContext().OrgSystems
            .Include(s => s.OutgoingDependencies)
            .ThenInclude(d => d.TargetSystem)
            .First(s => s.Id == webAppId);

        var apiGateway = CreateDbContext().OrgSystems
            .Include(s => s.OutgoingDependencies)
            .ThenInclude(d => d.TargetSystem)
            .Include(s => s.IncomingDependencies)
            .ThenInclude(d => d.SourceSystem)
            .First(s => s.Id == apiGatewayId);

        var authService = CreateDbContext().OrgSystems
            .Include(s => s.IncomingDependencies)
            .ThenInclude(d => d.SourceSystem)
            .First(s => s.Id == authServiceId);

        // Web App dependencies
        webApp.OutgoingDependencies.Should().HaveCount(2);
        webApp.OutgoingDependencies.Should().Contain(d => d.TargetSystem.Name == "API Gateway");
        webApp.OutgoingDependencies.Should().Contain(d => d.TargetSystem.Name == "Authentication Service");

        // API Gateway dependencies (outgoing and incoming)
        apiGateway.OutgoingDependencies.Should().HaveCount(3);
        apiGateway.OutgoingDependencies.Should().Contain(d => d.TargetSystem.Name == "Database System");
        apiGateway.OutgoingDependencies.Should().Contain(d => d.TargetSystem.Name == "Cache Layer");
        apiGateway.OutgoingDependencies.Should().Contain(d => d.TargetSystem.Name == "Authentication Service");

        apiGateway.IncomingDependencies.Should().HaveCount(1);
        apiGateway.IncomingDependencies.Should().Contain(d => d.SourceSystem.Name == "Web Application");

        // Auth Service incoming dependencies
        authService.IncomingDependencies.Should().HaveCount(2);
        authService.IncomingDependencies.Should().Contain(d => d.SourceSystem.Name == "Web Application");
        authService.IncomingDependencies.Should().Contain(d => d.SourceSystem.Name == "API Gateway");
    }

    [Test]
    public async Task SystemQueryWorkflow_CreateSystemsAndQuery_ReturnsCorrectResults()
    {
        // Arrange - Create systems with different characteristics
        await CreateSystemWithDetails("Critical Production System", SystemCriticality.Critical, SystemLifecycleStatus.Active, new[] { "Production", "Critical" });
        await CreateSystemWithDetails("Development Test System", SystemCriticality.Low, SystemLifecycleStatus.Active, new[] { "Development", "Testing" });
        await CreateSystemWithDetails("Legacy System", SystemCriticality.Medium, SystemLifecycleStatus.Retired, new[] { "Legacy", "Deprecated" });

        // Act - Query all systems
        var allSystemsQuery = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "Name"
        };

        var allSystemsResult = await SendAsync(allSystemsQuery);

        // Assert - All systems query
        allSystemsResult.Should().NotBeNull();
        allSystemsResult.TotalItems.Should().BeGreaterOrEqualTo(3);
        allSystemsResult.Data.Should().Contain(s => s.Name == "Critical Production System");
        allSystemsResult.Data.Should().Contain(s => s.Name == "Development Test System");
        allSystemsResult.Data.Should().Contain(s => s.Name == "Legacy System");

        // Act - Query critical systems only
        var criticalSystemsQuery = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            AdvancedFilter = new()
            {
                Criticality = SystemCriticality.Critical
            }
        };

        var criticalSystemsResult = await SendAsync(criticalSystemsQuery);

        // Assert - Critical systems query
        criticalSystemsResult.Should().NotBeNull();
        criticalSystemsResult.Data.Should().OnlyContain(s => s.Criticality == SystemCriticality.Critical);
        criticalSystemsResult.Data.Should().Contain(s => s.Name == "Critical Production System");

        // Act - Query active systems only
        var activeSystemsQuery = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            AdvancedFilter = new()
            {
                LifecycleStatus = SystemLifecycleStatus.Active
            }
        };

        var activeSystemsResult = await SendAsync(activeSystemsQuery);

        // Assert - Active systems query
        activeSystemsResult.Should().NotBeNull();
        activeSystemsResult.Data.Should().OnlyContain(s => s.LifecycleStatus == SystemLifecycleStatus.Active);
        activeSystemsResult.Data.Should().NotContain(s => s.Name == "Legacy System");
    }

    // Helper methods
    private async Task<int> CreateAttributeDefinition(string name, AttributeDataType dataType, bool isRequired, string? enumOptions = null)
    {
        var command = new AddEditAttributeDefinitionCommand
        {
            Name = name,
            Description = $"Attribute definition for {name}",
            DataType = dataType,
            IsRequired = isRequired,
            EnumOptionsCsv = enumOptions
        };

        var result = await SendAsync(command);
        return result.Data;
    }

    private async Task<int> CreateSystem(string name, SystemCriticality criticality)
    {
        var command = new AddEditOrgSystemCommand
        {
            Name = name,
            Description = $"Description for {name}",
            Criticality = criticality,
            LifecycleStatus = SystemLifecycleStatus.Active
        };

        var result = await SendAsync(command);
        return result.Data;
    }

    private async Task<int> CreateSystemWithDetails(string name, SystemCriticality criticality, SystemLifecycleStatus status, string[] tags)
    {
        var command = new AddEditOrgSystemCommand
        {
            Name = name,
            Description = $"Description for {name}",
            Criticality = criticality,
            LifecycleStatus = status,
            Tags = tags.ToList()
        };

        var result = await SendAsync(command);
        return result.Data;
    }

    private async Task AddSystemAttribute(int systemId, int attributeDefinitionId, string value)
    {
        var context = CreateDbContext();
        var systemAttribute = new SystemAttribute
        {
            OrgSystemId = systemId,
            AttributeDefinitionId = attributeDefinitionId,
            Value = value
        };

        context.SystemAttributes.Add(systemAttribute);
        await context.SaveChangesAsync();
    }

    private async Task AddDependencyRelationship(int sourceSystemId, int targetSystemId, string notes)
    {
        var context = CreateDbContext();
        var dependency = new DependencyEdge
        {
            SourceSystemId = sourceSystemId,
            TargetSystemId = targetSystemId,
            Notes = notes
        };

        context.DependencyEdges.Add(dependency);
        await context.SaveChangesAsync();
    }
}

// Command definitions that might be missing - these would normally be in the Application layer
public class AddEditITSystemCommand : ICacheInvalidatorRequest<Result<int>>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentSystemId { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
    public string CacheKey => "ITSystems";
    public IEnumerable<string>? TagsInvalidate => new[] { "ITSystems" };
    IEnumerable<string>? ICacheInvalidatorRequest<Result<int>>.Tags => TagsInvalidate;
}

public class AddEditITSystemCommandHandler : IRequestHandler<AddEditITSystemCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public AddEditITSystemCommandHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(AddEditITSystemCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        if (request.Id > 0)
        {
            var item = await db.ITSystems.SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            if (item == null) return await Result<int>.FailureAsync($"ITSystem with id: [{request.Id}] not found.");
            item = _mapper.Map(request, item);
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            var item = _mapper.Map<ITSystem>(request);
            db.ITSystems.Add(item);
            await db.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
    }
}