// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Enums;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Domain.UnitTests.Entities;

[TestFixture]
public class OrgSystemTests
{
    [Test]
    public void Create_OrgSystem_SetsDefaultValues()
    {
        // Act
        var system = new OrgSystem();

        // Assert
        system.Name.Should().Be(string.Empty);
        system.Criticality.Should().Be(SystemCriticality.Medium);
        system.LifecycleStatus.Should().Be(SystemLifecycleStatus.Active);
        system.Attributes.Should().NotBeNull().And.BeEmpty();
        system.OutgoingDependencies.Should().NotBeNull().And.BeEmpty();
        system.IncomingDependencies.Should().NotBeNull().And.BeEmpty();
    }

    [Test]
    public void Create_OrgSystem_WithRequiredProperties_SetsValuesCorrectly()
    {
        // Arrange
        var systemName = "HR Management System";
        var description = "System for managing human resources";

        // Act
        var system = new OrgSystem
        {
            Name = systemName,
            Description = description,
            BusinessOwnerId = 1,
            TechnicalOwnerId = 2,
            Criticality = SystemCriticality.High,
            LifecycleStatus = SystemLifecycleStatus.Active,
            Tags = new List<string> { "HR", "Management", "Core" }
        };

        // Assert
        system.Name.Should().Be(systemName);
        system.Description.Should().Be(description);
        system.BusinessOwnerId.Should().Be(1);
        system.TechnicalOwnerId.Should().Be(2);
        system.Criticality.Should().Be(SystemCriticality.High);
        system.LifecycleStatus.Should().Be(SystemLifecycleStatus.Active);
        system.Tags.Should().ContainInOrder("HR", "Management", "Core");
    }

    [Test]
    public void AddAttribute_ToOrgSystem_AddsSuccessfully()
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };
        var attributeDefinition = new AttributeDefinition 
        { 
            Name = "Version",
            DataType = CleanArchitecture.Blazor.Domain.Enums.AttributeDataType.Text,
            IsRequired = true
        };

        var systemAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDefinition,
            Value = "1.0.0"
        };

        // Act
        system.Attributes.Add(systemAttribute);

        // Assert
        system.Attributes.Should().HaveCount(1);
        system.Attributes.First().Value.Should().Be("1.0.0");
        system.Attributes.First().AttributeDefinition.Name.Should().Be("Version");
    }

    [Test]
    public void AddDependency_ToOrgSystem_CreatesBidirectionalRelationship()
    {
        // Arrange
        var sourceSystem = new OrgSystem { Name = "Source System" };
        var targetSystem = new OrgSystem { Name = "Target System" };

        var dependency = new DependencyEdge
        {
            SourceSystem = sourceSystem,
            TargetSystem = targetSystem,
            Notes = "API integration dependency"
        };

        // Act
        sourceSystem.OutgoingDependencies.Add(dependency);
        targetSystem.IncomingDependencies.Add(dependency);

        // Assert
        sourceSystem.OutgoingDependencies.Should().HaveCount(1);
        targetSystem.IncomingDependencies.Should().HaveCount(1);
        
        var outgoingDep = sourceSystem.OutgoingDependencies.First();
        var incomingDep = targetSystem.IncomingDependencies.First();
        
        outgoingDep.Should().BeSameAs(incomingDep);
        outgoingDep.Notes.Should().Be("API integration dependency");
    }

    [Test]
    public void SystemCriticality_ValidValues_AcceptsAllEnumValues()
    {
        // Arrange & Act & Assert
        var lowSystem = new OrgSystem { Criticality = SystemCriticality.Low };
        var mediumSystem = new OrgSystem { Criticality = SystemCriticality.Medium };
        var highSystem = new OrgSystem { Criticality = SystemCriticality.High };
        var criticalSystem = new OrgSystem { Criticality = SystemCriticality.Critical };

        lowSystem.Criticality.Should().Be(SystemCriticality.Low);
        mediumSystem.Criticality.Should().Be(SystemCriticality.Medium);
        highSystem.Criticality.Should().Be(SystemCriticality.High);
        criticalSystem.Criticality.Should().Be(SystemCriticality.Critical);
    }

    [Test]
    public void SystemLifecycleStatus_ValidValues_AcceptsAllEnumValues()
    {
        // Arrange & Act & Assert
        var activeSystem = new OrgSystem { LifecycleStatus = SystemLifecycleStatus.Active };
        var inactiveSystem = new OrgSystem { LifecycleStatus = SystemLifecycleStatus.Inactive };
        var retiredSystem = new OrgSystem { LifecycleStatus = SystemLifecycleStatus.Retired };

        activeSystem.LifecycleStatus.Should().Be(SystemLifecycleStatus.Active);
        inactiveSystem.LifecycleStatus.Should().Be(SystemLifecycleStatus.Inactive);
        retiredSystem.LifecycleStatus.Should().Be(SystemLifecycleStatus.Retired);
    }

    [Test]
    public void Tags_CollectionOperations_WorkCorrectly()
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };

        // Act
        system.Tags = new List<string> { "Initial", "Tag" };
        system.Tags.Add("NewTag");
        system.Tags.Remove("Initial");

        // Assert
        system.Tags.Should().HaveCount(2);
        system.Tags.Should().Contain("Tag");
        system.Tags.Should().Contain("NewTag");
        system.Tags.Should().NotContain("Initial");
    }
}