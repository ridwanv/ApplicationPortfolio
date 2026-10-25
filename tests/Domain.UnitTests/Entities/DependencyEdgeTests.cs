// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Enums;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Domain.UnitTests.Entities;

[TestFixture]
public class DependencyEdgeTests
{
    [Test]
    public void Create_DependencyEdge_SetsDefaultValues()
    {
        // Act
        var dependency = new DependencyEdge();

        // Assert
        dependency.SourceSystemId.Should().Be(0);
        dependency.TargetSystemId.Should().Be(0);
        dependency.Notes.Should().BeNull();
        dependency.SourceSystem.Should().BeNull();
        dependency.TargetSystem.Should().BeNull();
    }

    [Test]
    public void Create_DependencyEdge_WithSystems_EstablishesRelationship()
    {
        // Arrange
        var sourceSystem = new OrgSystem 
        { 
            Name = "Source System",
            Criticality = SystemCriticality.High
        };
        
        var targetSystem = new OrgSystem 
        { 
            Name = "Target System", 
            Criticality = SystemCriticality.Medium
        };

        // Act
        var dependency = new DependencyEdge
        {
            SourceSystemId = 1,
            TargetSystemId = 2,
            SourceSystem = sourceSystem,
            TargetSystem = targetSystem,
            Notes = "API dependency for user authentication"
        };

        // Assert
        dependency.SourceSystemId.Should().Be(1);
        dependency.TargetSystemId.Should().Be(2);
        dependency.SourceSystem.Should().Be(sourceSystem);
        dependency.TargetSystem.Should().Be(targetSystem);
        dependency.Notes.Should().Be("API dependency for user authentication");
    }

    [Test]
    public void DependencyEdge_BidirectionalNavigation_WorksCorrectly()
    {
        // Arrange
        var system1 = new OrgSystem { Name = "System A" };
        var system2 = new OrgSystem { Name = "System B" };
        var system3 = new OrgSystem { Name = "System C" };

        // Act - Create dependencies: A -> B, B -> C, C -> A (circular)
        var dep1 = new DependencyEdge
        {
            SourceSystem = system1,
            TargetSystem = system2,
            Notes = "A depends on B"
        };

        var dep2 = new DependencyEdge
        {
            SourceSystem = system2,
            TargetSystem = system3,
            Notes = "B depends on C"
        };

        var dep3 = new DependencyEdge
        {
            SourceSystem = system3,
            TargetSystem = system1,
            Notes = "C depends on A"
        };

        system1.OutgoingDependencies.Add(dep1);
        system1.IncomingDependencies.Add(dep3);

        system2.IncomingDependencies.Add(dep1);
        system2.OutgoingDependencies.Add(dep2);

        system3.IncomingDependencies.Add(dep2);
        system3.OutgoingDependencies.Add(dep3);

        // Assert
        system1.OutgoingDependencies.Should().HaveCount(1);
        system1.IncomingDependencies.Should().HaveCount(1);
        system1.OutgoingDependencies.First().TargetSystem.Name.Should().Be("System B");
        system1.IncomingDependencies.First().SourceSystem.Name.Should().Be("System C");

        system2.OutgoingDependencies.Should().HaveCount(1);
        system2.IncomingDependencies.Should().HaveCount(1);
        system2.OutgoingDependencies.First().TargetSystem.Name.Should().Be("System C");
        system2.IncomingDependencies.First().SourceSystem.Name.Should().Be("System A");

        system3.OutgoingDependencies.Should().HaveCount(1);
        system3.IncomingDependencies.Should().HaveCount(1);
        system3.OutgoingDependencies.First().TargetSystem.Name.Should().Be("System A");
        system3.IncomingDependencies.First().SourceSystem.Name.Should().Be("System B");
    }

    [Test]
    public void DependencyEdge_WithDetailedNotes_StoresInformation()
    {
        // Arrange
        var webSystem = new OrgSystem { Name = "Web Application" };
        var dbSystem = new OrgSystem { Name = "Database System" };
        var detailedNotes = @"
            Type: Database Connection
            Protocol: SQL Server Native Client
            Port: 1433
            Connection Pool: Enabled
            Failover: Enabled with secondary replica
            SLA: 99.9% uptime requirement
        ";

        // Act
        var dependency = new DependencyEdge
        {
            SourceSystem = webSystem,
            TargetSystem = dbSystem,
            Notes = detailedNotes.Trim()
        };

        // Assert
        dependency.Notes.Should().Contain("Database Connection");
        dependency.Notes.Should().Contain("1433");
        dependency.Notes.Should().Contain("99.9% uptime");
        dependency.SourceSystem.Name.Should().Be("Web Application");
        dependency.TargetSystem.Name.Should().Be("Database System");
    }

    [Test]
    public void DependencyEdge_SelfReference_IsValid()
    {
        // Arrange
        var system = new OrgSystem { Name = "Distributed System" };

        // Act
        var selfDependency = new DependencyEdge
        {
            SourceSystem = system,
            TargetSystem = system,
            Notes = "Internal component dependency within distributed system"
        };

        // Assert
        selfDependency.SourceSystem.Should().Be(selfDependency.TargetSystem);
        selfDependency.Notes.Should().Contain("Internal component dependency");
    }

    [Test]
    public void DependencyEdge_MultipleRelationships_BetweenSameSystems()
    {
        // Arrange
        var crmSystem = new OrgSystem { Name = "CRM System" };
        var emailSystem = new OrgSystem { Name = "Email Marketing System" };

        // Act
        var dataDependency = new DependencyEdge
        {
            SourceSystem = crmSystem,
            TargetSystem = emailSystem,
            Notes = "Customer data synchronization"
        };

        var eventDependency = new DependencyEdge
        {
            SourceSystem = emailSystem,
            TargetSystem = crmSystem,
            Notes = "Email engagement event callbacks"
        };

        crmSystem.OutgoingDependencies.Add(dataDependency);
        crmSystem.IncomingDependencies.Add(eventDependency);
        
        emailSystem.IncomingDependencies.Add(dataDependency);
        emailSystem.OutgoingDependencies.Add(eventDependency);

        // Assert
        crmSystem.OutgoingDependencies.Should().HaveCount(1);
        crmSystem.IncomingDependencies.Should().HaveCount(1);
        
        emailSystem.OutgoingDependencies.Should().HaveCount(1);
        emailSystem.IncomingDependencies.Should().HaveCount(1);

        crmSystem.OutgoingDependencies.First().Notes.Should().Contain("synchronization");
        crmSystem.IncomingDependencies.First().Notes.Should().Contain("callbacks");
    }

    [Test]
    public void DependencyEdge_WithoutNotes_IsValid()
    {
        // Arrange
        var system1 = new OrgSystem { Name = "System One" };
        var system2 = new OrgSystem { Name = "System Two" };

        // Act
        var dependency = new DependencyEdge
        {
            SourceSystem = system1,
            TargetSystem = system2
            // Notes intentionally left null
        };

        // Assert
        dependency.SourceSystem.Should().NotBeNull();
        dependency.TargetSystem.Should().NotBeNull();
        dependency.Notes.Should().BeNull();
    }

    [Test]
    public void DependencyEdge_SystemIds_MatchNavigationProperties()
    {
        // Arrange
        var sourceSystem = new OrgSystem { Name = "Source" };
        sourceSystem.Id = 100; // Simulating entity framework assignment
        
        var targetSystem = new OrgSystem { Name = "Target" };
        targetSystem.Id = 200; // Simulating entity framework assignment

        // Act
        var dependency = new DependencyEdge
        {
            SourceSystemId = sourceSystem.Id,
            TargetSystemId = targetSystem.Id,
            SourceSystem = sourceSystem,
            TargetSystem = targetSystem,
            Notes = "Test dependency"
        };

        // Assert
        dependency.SourceSystemId.Should().Be(dependency.SourceSystem.Id);
        dependency.TargetSystemId.Should().Be(dependency.TargetSystem.Id);
        dependency.SourceSystemId.Should().Be(100);
        dependency.TargetSystemId.Should().Be(200);
    }

    [Test]
    public void ComplexDependencyGraph_Navigation_WorksCorrectly()
    {
        // Arrange - Create a realistic dependency scenario
        var loadBalancer = new OrgSystem { Name = "Load Balancer" };
        var webServer1 = new OrgSystem { Name = "Web Server 1" };
        var webServer2 = new OrgSystem { Name = "Web Server 2" };
        var apiGateway = new OrgSystem { Name = "API Gateway" };
        var database = new OrgSystem { Name = "Database" };

        // Act - Create dependency relationships
        var dependencies = new List<DependencyEdge>
        {
            new() { SourceSystem = loadBalancer, TargetSystem = webServer1, Notes = "Load distribution" },
            new() { SourceSystem = loadBalancer, TargetSystem = webServer2, Notes = "Load distribution" },
            new() { SourceSystem = webServer1, TargetSystem = apiGateway, Notes = "API calls" },
            new() { SourceSystem = webServer2, TargetSystem = apiGateway, Notes = "API calls" },
            new() { SourceSystem = apiGateway, TargetSystem = database, Notes = "Data access" }
        };

        // Wire up the relationships
        foreach (var dep in dependencies)
        {
            dep.SourceSystem.OutgoingDependencies.Add(dep);
            dep.TargetSystem.IncomingDependencies.Add(dep);
        }

        // Assert
        loadBalancer.OutgoingDependencies.Should().HaveCount(2);
        loadBalancer.IncomingDependencies.Should().BeEmpty();

        webServer1.OutgoingDependencies.Should().HaveCount(1);
        webServer1.IncomingDependencies.Should().HaveCount(1);

        apiGateway.OutgoingDependencies.Should().HaveCount(1);
        apiGateway.IncomingDependencies.Should().HaveCount(2);

        database.OutgoingDependencies.Should().BeEmpty();
        database.IncomingDependencies.Should().HaveCount(1);

        // Verify specific relationships
        loadBalancer.OutgoingDependencies.Should().Contain(dep => dep.TargetSystem.Name == "Web Server 1");
        loadBalancer.OutgoingDependencies.Should().Contain(dep => dep.TargetSystem.Name == "Web Server 2");
        
        apiGateway.IncomingDependencies.Should().Contain(dep => dep.SourceSystem.Name == "Web Server 1");
        apiGateway.IncomingDependencies.Should().Contain(dep => dep.SourceSystem.Name == "Web Server 2");
    }
}