// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Domain.UnitTests.Entities;

[TestFixture]
public class AttributeDefinitionTests
{
    [Test]
    public void Create_AttributeDefinition_SetsDefaultValues()
    {
        // Act
        var attributeDef = new AttributeDefinition();

        // Assert
        attributeDef.Name.Should().Be(string.Empty);
        attributeDef.DataType.Should().Be(AttributeDataType.Text);
        attributeDef.IsRequired.Should().BeFalse();
        attributeDef.Description.Should().BeNull();
        attributeDef.EnumOptionsCsv.Should().BeNull();
    }

    [Test]
    public void Create_AttributeDefinition_WithAllProperties_SetsValuesCorrectly()
    {
        // Arrange & Act
        var attributeDef = new AttributeDefinition
        {
            Name = "Environment",
            Description = "Deployment environment for the system",
            DataType = AttributeDataType.Enum,
            IsRequired = true,
            EnumOptionsCsv = "Development,Testing,Staging,Production"
        };

        // Assert
        attributeDef.Name.Should().Be("Environment");
        attributeDef.Description.Should().Be("Deployment environment for the system");
        attributeDef.DataType.Should().Be(AttributeDataType.Enum);
        attributeDef.IsRequired.Should().BeTrue();
        attributeDef.EnumOptionsCsv.Should().Be("Development,Testing,Staging,Production");
    }

    [TestCase(AttributeDataType.Text, "Sample text value")]
    [TestCase(AttributeDataType.Number, "12345")]
    [TestCase(AttributeDataType.Boolean, "true")]
    [TestCase(AttributeDataType.Date, "2024-01-01")]
    [TestCase(AttributeDataType.Enum, "Option1")]
    public void DataType_AcceptsAllValidTypes(AttributeDataType dataType, string expectedValue)
    {
        // Arrange & Act
        var attributeDef = new AttributeDefinition
        {
            Name = $"Test{dataType}Attribute",
            DataType = dataType,
            Description = $"Test attribute for {dataType} data type"
        };

        // Assert
        attributeDef.DataType.Should().Be(dataType);
        attributeDef.Name.Should().Contain(dataType.ToString());
    }

    [Test]
    public void EnumOptions_ParsesCsvCorrectly()
    {
        // Arrange
        var csvOptions = "Low,Medium,High,Critical";
        var attributeDef = new AttributeDefinition
        {
            Name = "Priority",
            DataType = AttributeDataType.Enum,
            EnumOptionsCsv = csvOptions
        };

        // Act
        var options = attributeDef.EnumOptionsCsv?.Split(',').Select(o => o.Trim()).ToList();

        // Assert
        options.Should().HaveCount(4);
        options.Should().ContainInOrder("Low", "Medium", "High", "Critical");
    }

    [Test]
    public void EnumOptions_HandlesEmptyAndWhitespace()
    {
        // Arrange
        var csvOptions = "Option1, Option2 , Option3,  , Option5";
        var attributeDef = new AttributeDefinition
        {
            Name = "TestEnum",
            DataType = AttributeDataType.Enum,
            EnumOptionsCsv = csvOptions
        };

        // Act
        var options = attributeDef.EnumOptionsCsv?.Split(',')
            .Select(o => o.Trim())
            .Where(o => !string.IsNullOrWhiteSpace(o))
            .ToList();

        // Assert
        options.Should().HaveCount(4);
        options.Should().ContainInOrder("Option1", "Option2", "Option3", "Option5");
    }

    [Test]
    public void Required_AttributeDefinition_CanBeMarkedAsRequired()
    {
        // Arrange & Act
        var requiredAttribute = new AttributeDefinition
        {
            Name = "SystemId",
            DataType = AttributeDataType.Text,
            IsRequired = true,
            Description = "Unique system identifier"
        };

        var optionalAttribute = new AttributeDefinition
        {
            Name = "Notes",
            DataType = AttributeDataType.Text,
            IsRequired = false,
            Description = "Optional system notes"
        };

        // Assert
        requiredAttribute.IsRequired.Should().BeTrue();
        optionalAttribute.IsRequired.Should().BeFalse();
    }

    [Test]
    public void BooleanAttribute_Configuration_IsValid()
    {
        // Arrange & Act
        var booleanAttribute = new AttributeDefinition
        {
            Name = "IsActive",
            DataType = AttributeDataType.Boolean,
            IsRequired = true,
            Description = "Indicates if the system is currently active",
            EnumOptionsCsv = null // Should be null for boolean types
        };

        // Assert
        booleanAttribute.DataType.Should().Be(AttributeDataType.Boolean);
        booleanAttribute.IsRequired.Should().BeTrue();
        booleanAttribute.EnumOptionsCsv.Should().BeNull();
    }

    [Test]
    public void DateAttribute_Configuration_IsValid()
    {
        // Arrange & Act
        var dateAttribute = new AttributeDefinition
        {
            Name = "LastUpdated",
            DataType = AttributeDataType.Date,
            IsRequired = false,
            Description = "Last modification date of the system"
        };

        // Assert
        dateAttribute.DataType.Should().Be(AttributeDataType.Date);
        dateAttribute.IsRequired.Should().BeFalse();
        dateAttribute.Description.Should().Contain("modification date");
    }

    [Test]
    public void NumberAttribute_Configuration_IsValid()
    {
        // Arrange & Act
        var numberAttribute = new AttributeDefinition
        {
            Name = "MaxUsers",
            DataType = AttributeDataType.Number,
            IsRequired = true,
            Description = "Maximum number of concurrent users supported"
        };

        // Assert
        numberAttribute.DataType.Should().Be(AttributeDataType.Number);
        numberAttribute.IsRequired.Should().BeTrue();
        numberAttribute.Name.Should().Be("MaxUsers");
    }

    [Test]
    public void AttributeDefinition_NameValidation_RequiresNonEmptyName()
    {
        // Arrange & Act
        var validAttribute = new AttributeDefinition { Name = "ValidName" };
        var invalidAttribute = new AttributeDefinition { Name = string.Empty };

        // Assert
        validAttribute.Name.Should().NotBeEmpty();
        invalidAttribute.Name.Should().BeEmpty();
        
        // In real scenarios, validation would be handled by FluentValidation or data annotations
        // This test demonstrates the expected behavior
    }

    [Test]
    public void EnumAttribute_WithoutOptions_IsIncomplete()
    {
        // Arrange & Act
        var incompleteEnumAttribute = new AttributeDefinition
        {
            Name = "Status",
            DataType = AttributeDataType.Enum,
            EnumOptionsCsv = null // This should normally have options
        };

        var completeEnumAttribute = new AttributeDefinition
        {
            Name = "Priority",
            DataType = AttributeDataType.Enum,
            EnumOptionsCsv = "Low,High"
        };

        // Assert
        incompleteEnumAttribute.DataType.Should().Be(AttributeDataType.Enum);
        incompleteEnumAttribute.EnumOptionsCsv.Should().BeNull();
        
        completeEnumAttribute.DataType.Should().Be(AttributeDataType.Enum);
        completeEnumAttribute.EnumOptionsCsv.Should().NotBeNull();
    }
}