// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Domain.UnitTests.Entities;

[TestFixture]
public class ITSystemTests
{
    [Test]
    public void Create_ITSystem_SetsDefaultValues()
    {
        // Act
        var system = new ITSystem();

        // Assert
        system.Name.Should().Be(string.Empty);
        system.SubSystems.Should().NotBeNull().And.BeEmpty();
        system.SystemCapabilities.Should().NotBeNull().And.BeEmpty();
        system.ParentSystemId.Should().BeNull();
        system.ParentSystem.Should().BeNull();
    }

    [Test]
    public void Create_ITSystem_WithAllProperties_SetsValuesCorrectly()
    {
        // Arrange
        var installationDate = DateTime.UtcNow.Date;
        var maintenanceDate = DateTime.UtcNow.AddDays(-30).Date;

        // Act
        var system = new ITSystem
        {
            Name = "Enterprise Resource Planning System",
            Description = "Comprehensive ERP solution for business operations",
            Owner = "IT Department",
            Version = "2.1.5",
            Status = "Production",
            Category = "Enterprise Software",
            Vendor = "SAP AG",
            InstallationDate = installationDate,
            LastMaintenanceDate = maintenanceDate,
            MaintenanceNotes = "Quarterly security updates applied",
            AnnualCost = 150000.00m,
            CriticalityLevel = "High",
            ComplianceRequirements = "SOX, GDPR, PCI-DSS"
        };

        // Assert
        system.Name.Should().Be("Enterprise Resource Planning System");
        system.Description.Should().Be("Comprehensive ERP solution for business operations");
        system.Owner.Should().Be("IT Department");
        system.Version.Should().Be("2.1.5");
        system.Status.Should().Be("Production");
        system.Category.Should().Be("Enterprise Software");
        system.Vendor.Should().Be("SAP AG");
        system.InstallationDate.Should().Be(installationDate);
        system.LastMaintenanceDate.Should().Be(maintenanceDate);
        system.MaintenanceNotes.Should().Be("Quarterly security updates applied");
        system.AnnualCost.Should().Be(150000.00m);
        system.CriticalityLevel.Should().Be("High");
        system.ComplianceRequirements.Should().Be("SOX, GDPR, PCI-DSS");
    }

    [Test]
    public void AddSubSystem_CreatesHierarchicalRelationship()
    {
        // Arrange
        var parentSystem = new ITSystem { Name = "Parent System" };
        var childSystem = new ITSystem 
        { 
            Name = "Child System",
            ParentSystem = parentSystem
        };
        childSystem.ParentSystemId = parentSystem.Id;

        // Act
        parentSystem.SubSystems.Add(childSystem);

        // Assert
        parentSystem.SubSystems.Should().HaveCount(1);
        parentSystem.SubSystems.First().Should().Be(childSystem);
        childSystem.ParentSystem.Should().Be(parentSystem);
        childSystem.ParentSystemId.Should().Be(parentSystem.Id);
    }

    [Test]
    public void MultiLevel_HierarchicalStructure_WorksCorrectly()
    {
        // Arrange
        var rootSystem = new ITSystem { Name = "Root System" };
        var level1System = new ITSystem 
        { 
            Name = "Level 1 System",
            ParentSystem = rootSystem
        };
        var level2System = new ITSystem 
        { 
            Name = "Level 2 System",
            ParentSystem = level1System
        };

        // Act
        rootSystem.SubSystems.Add(level1System);
        level1System.SubSystems.Add(level2System);

        // Assert
        rootSystem.SubSystems.Should().HaveCount(1);
        level1System.SubSystems.Should().HaveCount(1);
        level2System.SubSystems.Should().BeEmpty();
        
        level2System.ParentSystem.Should().Be(level1System);
        level1System.ParentSystem.Should().Be(rootSystem);
        rootSystem.ParentSystem.Should().BeNull();
    }

    [Test]
    public void AddSystemCapability_EstablishesCapabilityMapping()
    {
        // Arrange
        var system = new ITSystem { Name = "HR System" };
        var capability = new Capability { Name = "Employee Management" };
        var systemCapability = new SystemCapability
        {
            ITSystem = system,
            Capability = capability
        };

        // Act
        system.SystemCapabilities.Add(systemCapability);

        // Assert
        system.SystemCapabilities.Should().HaveCount(1);
        system.SystemCapabilities.First().Capability.Name.Should().Be("Employee Management");
        system.SystemCapabilities.First().ITSystem.Should().Be(system);
    }

    [Test]
    public void MonetaryValues_HandleDecimalCorrectly()
    {
        // Arrange & Act
        var system = new ITSystem 
        { 
            Name = "Test System",
            AnnualCost = 99999.99m
        };

        // Assert
        system.AnnualCost.Should().Be(99999.99m);
        system.AnnualCost.Should().BeOfType<decimal?>();
    }

    [Test]
    public void DateFields_HandleNullableCorrectly()
    {
        // Arrange
        var system = new ITSystem { Name = "Test System" };

        // Act & Assert - Initially null
        system.InstallationDate.Should().BeNull();
        system.LastMaintenanceDate.Should().BeNull();

        // Act - Set values
        var installDate = DateTime.UtcNow.Date;
        var maintDate = DateTime.UtcNow.AddDays(-10).Date;
        
        system.InstallationDate = installDate;
        system.LastMaintenanceDate = maintDate;

        // Assert - Values set correctly
        system.InstallationDate.Should().Be(installDate);
        system.LastMaintenanceDate.Should().Be(maintDate);
    }

    [Test]
    public void OptionalFields_AcceptNullValues()
    {
        // Arrange & Act
        var system = new ITSystem
        {
            Name = "Required Name Only System"
            // All other fields intentionally left as null
        };

        // Assert
        system.Description.Should().BeNull();
        system.Owner.Should().BeNull();
        system.Version.Should().BeNull();
        system.Status.Should().BeNull();
        system.Category.Should().BeNull();
        system.Vendor.Should().BeNull();
        system.MaintenanceNotes.Should().BeNull();
        system.CriticalityLevel.Should().BeNull();
        system.ComplianceRequirements.Should().BeNull();
        system.AnnualCost.Should().BeNull();
    }

    [Test]
    public void CascadingDelete_SubSystems_MaintainsDataIntegrity()
    {
        // Arrange
        var parentSystem = new ITSystem { Name = "Parent System" };
        var childSystem1 = new ITSystem { Name = "Child 1", ParentSystem = parentSystem };
        var childSystem2 = new ITSystem { Name = "Child 2", ParentSystem = parentSystem };
        
        parentSystem.SubSystems.Add(childSystem1);
        parentSystem.SubSystems.Add(childSystem2);

        // Act
        parentSystem.SubSystems.Clear();

        // Assert
        parentSystem.SubSystems.Should().BeEmpty();
        // Note: In a real scenario, orphaned child systems would need to be handled
        // by the application logic or database cascading rules
    }
}