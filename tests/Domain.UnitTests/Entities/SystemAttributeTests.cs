// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Enums;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Domain.UnitTests.Entities;

[TestFixture]
public class SystemAttributeTests
{
    [Test]
    public void Create_SystemAttribute_SetsDefaultValues()
    {
        // Act
        var systemAttribute = new SystemAttribute();

        // Assert
        systemAttribute.OrgSystemId.Should().Be(0);
        systemAttribute.AttributeDefinitionId.Should().Be(0);
        systemAttribute.Value.Should().BeNull();
        systemAttribute.OrgSystem.Should().BeNull();
        systemAttribute.AttributeDefinition.Should().BeNull();
    }

    [Test]
    public void Create_SystemAttribute_WithAllProperties_SetsValuesCorrectly()
    {
        // Arrange
        var orgSystem = new OrgSystem { Name = "Test System" };
        orgSystem.Id = 1;
        
        var attributeDef = new AttributeDefinition 
        { 
            Name = "Version",
            DataType = AttributeDataType.Text,
            IsRequired = true
        };
        attributeDef.Id = 1;

        // Act
        var systemAttribute = new SystemAttribute
        {
            OrgSystemId = orgSystem.Id,
            AttributeDefinitionId = attributeDef.Id,
            Value = "2.1.0",
            OrgSystem = orgSystem,
            AttributeDefinition = attributeDef
        };

        // Assert
        systemAttribute.OrgSystemId.Should().Be(1);
        systemAttribute.AttributeDefinitionId.Should().Be(1);
        systemAttribute.Value.Should().Be("2.1.0");
        systemAttribute.OrgSystem.Should().Be(orgSystem);
        systemAttribute.AttributeDefinition.Should().Be(attributeDef);
    }

    [TestCase(AttributeDataType.Text, "Sample text value")]
    [TestCase(AttributeDataType.Number, "12345")]
    [TestCase(AttributeDataType.Boolean, "true")]
    [TestCase(AttributeDataType.Date, "2024-01-01")]
    [TestCase(AttributeDataType.Enum, "Production")]
    public void SystemAttribute_SupportsAllDataTypes(AttributeDataType dataType, string value)
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };
        var attributeDef = new AttributeDefinition 
        { 
            Name = $"Test{dataType}",
            DataType = dataType
        };

        // Act
        var systemAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = value
        };

        // Assert
        systemAttribute.Value.Should().Be(value);
        systemAttribute.AttributeDefinition.DataType.Should().Be(dataType);
    }

    [Test]
    public void SystemAttribute_TextDataType_AcceptsLongValues()
    {
        // Arrange
        var longText = new string('A', 1000); // 1000 character string
        var system = new OrgSystem { Name = "Test System" };
        var attributeDef = new AttributeDefinition 
        { 
            Name = "LongDescription",
            DataType = AttributeDataType.Text
        };

        // Act
        var systemAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = longText
        };

        // Assert
        systemAttribute.Value.Should().Be(longText);
        systemAttribute.Value.Should().HaveLength(1000);
    }

    [Test]
    public void SystemAttribute_NumericDataType_StoresAsString()
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };
        var attributeDef = new AttributeDefinition 
        { 
            Name = "MaxUsers",
            DataType = AttributeDataType.Number
        };

        // Act
        var systemAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = "150" // Stored as string but represents a number
        };

        // Assert
        systemAttribute.Value.Should().Be("150");
        systemAttribute.AttributeDefinition.DataType.Should().Be(AttributeDataType.Number);
        
        // Verify it can be parsed as a number
        int.TryParse(systemAttribute.Value, out var numericValue).Should().BeTrue();
        numericValue.Should().Be(150);
    }

    [Test]
    public void SystemAttribute_BooleanDataType_AcceptsTrueFalseValues()
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };
        var attributeDef = new AttributeDefinition 
        { 
            Name = "IsActive",
            DataType = AttributeDataType.Boolean
        };

        // Act
        var trueAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = "true"
        };

        var falseAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = "false"
        };

        // Assert
        trueAttribute.Value.Should().Be("true");
        falseAttribute.Value.Should().Be("false");
        
        bool.TryParse(trueAttribute.Value, out var boolValue1).Should().BeTrue();
        bool.TryParse(falseAttribute.Value, out var boolValue2).Should().BeTrue();
        boolValue1.Should().BeTrue();
        boolValue2.Should().BeFalse();
    }

    [Test]
    public void SystemAttribute_DateDataType_AcceptsValidDateFormats()
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };
        var attributeDef = new AttributeDefinition 
        { 
            Name = "LastUpdated",
            DataType = AttributeDataType.Date
        };

        // Act
        var dateAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = "2024-03-15T10:30:00Z"
        };

        // Assert
        dateAttribute.Value.Should().Be("2024-03-15T10:30:00Z");
        DateTime.TryParse(dateAttribute.Value, out var dateValue).Should().BeTrue();
        dateValue.Year.Should().Be(2024);
        dateValue.Month.Should().Be(3);
        dateValue.Day.Should().Be(15);
    }

    [Test]
    public void SystemAttribute_EnumDataType_ValidatesAgainstOptions()
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };
        var attributeDef = new AttributeDefinition 
        { 
            Name = "Environment",
            DataType = AttributeDataType.Enum,
            EnumOptionsCsv = "Development,Testing,Staging,Production"
        };

        // Act
        var validEnumAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = "Production"
        };

        var invalidEnumAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = "InvalidEnvironment"
        };

        // Assert
        validEnumAttribute.Value.Should().Be("Production");
        invalidEnumAttribute.Value.Should().Be("InvalidEnvironment");
        
        // Verify validation logic (would be handled by application layer)
        var validOptions = attributeDef.EnumOptionsCsv.Split(',');
        validOptions.Should().Contain(validEnumAttribute.Value);
        validOptions.Should().NotContain(invalidEnumAttribute.Value);
    }

    [Test]
    public void SystemAttribute_NullValue_IsValid()
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };
        var attributeDef = new AttributeDefinition 
        { 
            Name = "OptionalField",
            DataType = AttributeDataType.Text,
            IsRequired = false
        };

        // Act
        var systemAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = null
        };

        // Assert
        systemAttribute.Value.Should().BeNull();
        systemAttribute.AttributeDefinition.IsRequired.Should().BeFalse();
    }

    [Test]
    public void SystemAttribute_MultipleAttributesOnSameSystem_AllowsCollection()
    {
        // Arrange
        var system = new OrgSystem { Name = "Enterprise System" };
        
        var versionAttr = new AttributeDefinition { Name = "Version", DataType = AttributeDataType.Text };
        var environmentAttr = new AttributeDefinition { Name = "Environment", DataType = AttributeDataType.Enum };
        var isActiveAttr = new AttributeDefinition { Name = "IsActive", DataType = AttributeDataType.Boolean };

        // Act
        var attributes = new List<SystemAttribute>
        {
            new() { OrgSystem = system, AttributeDefinition = versionAttr, Value = "3.2.1" },
            new() { OrgSystem = system, AttributeDefinition = environmentAttr, Value = "Production" },
            new() { OrgSystem = system, AttributeDefinition = isActiveAttr, Value = "true" }
        };

        foreach (var attr in attributes)
        {
            system.Attributes.Add(attr);
        }

        // Assert
        system.Attributes.Should().HaveCount(3);
        system.Attributes.Should().Contain(attr => attr.AttributeDefinition.Name == "Version");
        system.Attributes.Should().Contain(attr => attr.AttributeDefinition.Name == "Environment");
        system.Attributes.Should().Contain(attr => attr.AttributeDefinition.Name == "IsActive");

        var versionAttribute = system.Attributes.First(attr => attr.AttributeDefinition.Name == "Version");
        versionAttribute.Value.Should().Be("3.2.1");
    }

    [Test]
    public void SystemAttribute_NavigationProperties_WorkBidirectionally()
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };
        var attributeDef = new AttributeDefinition 
        { 
            Name = "TestAttribute",
            DataType = AttributeDataType.Text
        };

        // Act
        var systemAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = "test value"
        };

        system.Attributes.Add(systemAttribute);

        // Assert
        systemAttribute.OrgSystem.Should().Be(system);
        systemAttribute.AttributeDefinition.Should().Be(attributeDef);
        system.Attributes.Should().Contain(systemAttribute);
        system.Attributes.First().AttributeDefinition.Should().Be(attributeDef);
    }

    [Test]
    public void SystemAttribute_UpdateValue_ModifiesCorrectly()
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };
        var attributeDef = new AttributeDefinition 
        { 
            Name = "Status",
            DataType = AttributeDataType.Enum,
            EnumOptionsCsv = "Active,Inactive,Maintenance"
        };

        var systemAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = "Active"
        };

        // Act
        systemAttribute.Value = "Maintenance";

        // Assert
        systemAttribute.Value.Should().Be("Maintenance");
        systemAttribute.OrgSystem.Should().Be(system);
        systemAttribute.AttributeDefinition.Should().Be(attributeDef);
    }

    [Test]
    public void SystemAttribute_EmptyStringValue_IsValidButDifferentFromNull()
    {
        // Arrange
        var system = new OrgSystem { Name = "Test System" };
        var attributeDef = new AttributeDefinition 
        { 
            Name = "Notes",
            DataType = AttributeDataType.Text
        };

        // Act
        var nullAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = null
        };

        var emptyStringAttribute = new SystemAttribute
        {
            OrgSystem = system,
            AttributeDefinition = attributeDef,
            Value = string.Empty
        };

        // Assert
        nullAttribute.Value.Should().BeNull();
        emptyStringAttribute.Value.Should().Be(string.Empty);
        nullAttribute.Value.Should().NotBe(emptyStringAttribute.Value);
    }
}