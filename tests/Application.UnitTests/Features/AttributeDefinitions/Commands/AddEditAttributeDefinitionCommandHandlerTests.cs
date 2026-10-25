// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Commands.AddEdit;
using CleanArchitecture.Blazor.Domain.Entities;
using CleanArchitecture.Blazor.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Features.AttributeDefinitions.Commands;

[TestFixture]
public class AddEditAttributeDefinitionCommandHandlerTests
{
    private Mock<IApplicationDbContextFactory> _mockDbContextFactory;
    private Mock<IApplicationDbContext> _mockDbContext;
    private Mock<IMapper> _mockMapper;
    private AddEditAttributeDefinitionCommandHandler _handler;
    private Mock<DbSet<AttributeDefinition>> _mockDbSet;

    [SetUp]
    public void SetUp()
    {
        _mockDbContextFactory = new Mock<IApplicationDbContextFactory>();
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockMapper = new Mock<IMapper>();
        _mockDbSet = new Mock<DbSet<AttributeDefinition>>();

        _mockDbContext.Setup(x => x.AttributeDefinitions).Returns(_mockDbSet.Object);
        _mockDbContextFactory.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_mockDbContext.Object);

        _handler = new AddEditAttributeDefinitionCommandHandler(_mockDbContextFactory.Object, _mockMapper.Object);
    }

    [Test]
    public async Task Handle_CreateNewTextAttributeDefinition_ReturnsSuccessResult()
    {
        // Arrange
        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 0,
            Name = "SystemVersion",
            Description = "Version number of the system",
            DataType = AttributeDataType.Text,
            IsRequired = true,
            EnumOptionsCsv = null
        };

        var expectedAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = command.Name,
            Description = command.Description,
            DataType = command.DataType,
            IsRequired = command.IsRequired,
            EnumOptionsCsv = command.EnumOptionsCsv
        };

        _mockMapper.Setup(x => x.Map<AttributeDefinition>(command))
            .Returns(expectedAttributeDef);

        _mockDbSet.Setup(x => x.Add(It.IsAny<AttributeDefinition>()));
        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().Be(expectedAttributeDef.Id);

        _mockDbContextFactory.Verify(x => x.CreateAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(x => x.Map<AttributeDefinition>(command), Times.Once);
        _mockDbSet.Verify(x => x.Add(It.IsAny<AttributeDefinition>()), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_CreateNewEnumAttributeDefinition_ReturnsSuccessResult()
    {
        // Arrange
        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 0,
            Name = "Environment",
            Description = "Deployment environment",
            DataType = AttributeDataType.Enum,
            IsRequired = true,
            EnumOptionsCsv = "Development,Testing,Staging,Production"
        };

        var expectedAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = command.Name,
            Description = command.Description,
            DataType = command.DataType,
            IsRequired = command.IsRequired,
            EnumOptionsCsv = command.EnumOptionsCsv
        };

        _mockMapper.Setup(x => x.Map<AttributeDefinition>(command))
            .Returns(expectedAttributeDef);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().Be(expectedAttributeDef.Id);

        _mockMapper.Verify(x => x.Map<AttributeDefinition>(It.Is<AddEditAttributeDefinitionCommand>(c => 
            c.DataType == AttributeDataType.Enum && 
            c.EnumOptionsCsv.Contains("Production"))), Times.Once);
    }

    [Test]
    public async Task Handle_CreateBooleanAttributeDefinition_ReturnsSuccessResult()
    {
        // Arrange
        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 0,
            Name = "IsActive",
            Description = "Indicates if the system is currently active",
            DataType = AttributeDataType.Boolean,
            IsRequired = true,
            EnumOptionsCsv = null // Should be null for boolean types
        };

        var expectedAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = command.Name,
            Description = command.Description,
            DataType = command.DataType,
            IsRequired = command.IsRequired,
            EnumOptionsCsv = null
        };

        _mockMapper.Setup(x => x.Map<AttributeDefinition>(command))
            .Returns(expectedAttributeDef);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockMapper.Verify(x => x.Map<AttributeDefinition>(It.Is<AddEditAttributeDefinitionCommand>(c => 
            c.DataType == AttributeDataType.Boolean && 
            c.EnumOptionsCsv == null)), Times.Once);
    }

    [Test]
    public async Task Handle_CreateNumberAttributeDefinition_ReturnsSuccessResult()
    {
        // Arrange
        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 0,
            Name = "MaxUsers",
            Description = "Maximum number of concurrent users",
            DataType = AttributeDataType.Number,
            IsRequired = false,
            EnumOptionsCsv = null
        };

        var expectedAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = command.Name,
            Description = command.Description,
            DataType = command.DataType,
            IsRequired = command.IsRequired,
            EnumOptionsCsv = null
        };

        _mockMapper.Setup(x => x.Map<AttributeDefinition>(command))
            .Returns(expectedAttributeDef);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().Be(expectedAttributeDef.Id);

        _mockMapper.Verify(x => x.Map<AttributeDefinition>(It.Is<AddEditAttributeDefinitionCommand>(c => 
            c.DataType == AttributeDataType.Number && 
            c.IsRequired == false)), Times.Once);
    }

    [Test]
    public async Task Handle_CreateDateAttributeDefinition_ReturnsSuccessResult()
    {
        // Arrange
        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 0,
            Name = "LastUpdated",
            Description = "Date when the system was last updated",
            DataType = AttributeDataType.Date,
            IsRequired = false,
            EnumOptionsCsv = null
        };

        var expectedAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = command.Name,
            Description = command.Description,
            DataType = command.DataType,
            IsRequired = command.IsRequired,
            EnumOptionsCsv = null
        };

        _mockMapper.Setup(x => x.Map<AttributeDefinition>(command))
            .Returns(expectedAttributeDef);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockMapper.Verify(x => x.Map<AttributeDefinition>(It.Is<AddEditAttributeDefinitionCommand>(c => 
            c.DataType == AttributeDataType.Date)), Times.Once);
    }

    [Test]
    public async Task Handle_UpdateExistingAttributeDefinition_ReturnsSuccessResult()
    {
        // Arrange
        var existingAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = "OldName",
            Description = "Old Description",
            DataType = AttributeDataType.Text,
            IsRequired = false
        };

        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 1,
            Name = "UpdatedName",
            Description = "Updated Description",
            DataType = AttributeDataType.Enum,
            IsRequired = true,
            EnumOptionsCsv = "Option1,Option2,Option3"
        };

        var updatedAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = command.Name,
            Description = command.Description,
            DataType = command.DataType,
            IsRequired = command.IsRequired,
            EnumOptionsCsv = command.EnumOptionsCsv
        };

        _mockDbSet.Setup(x => x.SingleOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<AttributeDefinition, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAttributeDef);

        _mockMapper.Setup(x => x.Map(command, existingAttributeDef))
            .Returns(updatedAttributeDef);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().Be(existingAttributeDef.Id);

        _mockDbSet.Verify(x => x.SingleOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<System.Func<AttributeDefinition, bool>>>(),
            It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(x => x.Map(command, existingAttributeDef), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_UpdateNonExistentAttributeDefinition_ReturnsFailureResult()
    {
        // Arrange
        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 999,
            Name = "Non-existent Attribute"
        };

        _mockDbSet.Setup(x => x.SingleOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<AttributeDefinition, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((AttributeDefinition)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.ErrorMessage.Should().Contain("not found");
        result.ErrorMessage.Should().Contain("999");

        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_CreateEnumWithComplexOptions_HandlesCorrectly()
    {
        // Arrange
        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 0,
            Name = "ComplexStatus",
            Description = "System status with complex enum options",
            DataType = AttributeDataType.Enum,
            IsRequired = true,
            EnumOptionsCsv = "Active - Running,Inactive - Stopped,Maintenance - Scheduled Downtime,Deprecated - End of Life,Unknown - Status Not Available"
        };

        var expectedAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = command.Name,
            Description = command.Description,
            DataType = command.DataType,
            IsRequired = command.IsRequired,
            EnumOptionsCsv = command.EnumOptionsCsv
        };

        _mockMapper.Setup(x => x.Map<AttributeDefinition>(command))
            .Returns(expectedAttributeDef);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockMapper.Verify(x => x.Map<AttributeDefinition>(It.Is<AddEditAttributeDefinitionCommand>(c => 
            c.EnumOptionsCsv.Contains("Active - Running") && 
            c.EnumOptionsCsv.Contains("Maintenance - Scheduled Downtime"))), Times.Once);
    }

    [Test]
    public async Task Handle_CancellationRequested_PropagatesCancellation()
    {
        // Arrange
        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 0,
            Name = "Test Attribute"
        };

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _mockDbContextFactory.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await FluentActions.Invoking(async () => 
            await _handler.Handle(command, cancellationTokenSource.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task Handle_DatabaseException_PropagatesException()
    {
        // Arrange
        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 0,
            Name = "Test Attribute"
        };

        var expectedAttributeDef = new AttributeDefinition { Name = command.Name };

        _mockMapper.Setup(x => x.Map<AttributeDefinition>(command))
            .Returns(expectedAttributeDef);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Duplicate attribute name"));

        // Act & Assert
        await FluentActions.Invoking(async () => 
            await _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Duplicate attribute name");
    }

    [Test]
    public async Task Handle_UpdateDataTypeFromTextToEnum_RequiresEnumOptions()
    {
        // Arrange
        var existingAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = "Status",
            DataType = AttributeDataType.Text,
            IsRequired = false,
            EnumOptionsCsv = null
        };

        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 1,
            Name = "Status",
            DataType = AttributeDataType.Enum,
            IsRequired = true,
            EnumOptionsCsv = "Active,Inactive,Maintenance"
        };

        var updatedAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = command.Name,
            DataType = command.DataType,
            IsRequired = command.IsRequired,
            EnumOptionsCsv = command.EnumOptionsCsv
        };

        _mockDbSet.Setup(x => x.SingleOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<AttributeDefinition, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAttributeDef);

        _mockMapper.Setup(x => x.Map(command, existingAttributeDef))
            .Returns(updatedAttributeDef);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockMapper.Verify(x => x.Map(It.Is<AddEditAttributeDefinitionCommand>(c => 
            c.DataType == AttributeDataType.Enum && 
            !string.IsNullOrEmpty(c.EnumOptionsCsv)), existingAttributeDef), Times.Once);
    }

    [Test]
    public async Task Handle_UpdateRequiredFlag_HandlesCorrectly()
    {
        // Arrange
        var existingAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = "TestAttribute",
            IsRequired = false
        };

        var command = new AddEditAttributeDefinitionCommand
        {
            Id = 1,
            Name = "TestAttribute",
            IsRequired = true // Changed from false to true
        };

        var updatedAttributeDef = new AttributeDefinition
        {
            Id = 1,
            Name = command.Name,
            IsRequired = command.IsRequired
        };

        _mockDbSet.Setup(x => x.SingleOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<System.Func<AttributeDefinition, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAttributeDef);

        _mockMapper.Setup(x => x.Map(command, existingAttributeDef))
            .Returns(updatedAttributeDef);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockMapper.Verify(x => x.Map(It.Is<AddEditAttributeDefinitionCommand>(c => 
            c.IsRequired == true), existingAttributeDef), Times.Once);
    }
}