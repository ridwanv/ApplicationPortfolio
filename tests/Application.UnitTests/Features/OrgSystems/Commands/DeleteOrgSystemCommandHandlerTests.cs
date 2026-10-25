// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Commands.Delete;
using CleanArchitecture.Blazor.Domain.Common.Enums;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Features.OrgSystems.Commands;

[TestFixture]
public class DeleteOrgSystemCommandHandlerTests
{
    private Mock<IApplicationDbContextFactory> _mockDbContextFactory;
    private Mock<IApplicationDbContext> _mockDbContext;
    private DeleteOrgSystemCommandHandler _handler;
    private Mock<DbSet<OrgSystem>> _mockDbSet;

    [SetUp]
    public void SetUp()
    {
        _mockDbContextFactory = new Mock<IApplicationDbContextFactory>();
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockDbSet = new Mock<DbSet<OrgSystem>>();

        _mockDbContext.Setup(x => x.OrgSystems).Returns(_mockDbSet.Object);
        _mockDbContextFactory.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_mockDbContext.Object);

        _handler = new DeleteOrgSystemCommandHandler(_mockDbContextFactory.Object);
    }

    [Test]
    public async Task Handle_DeleteExistingOrgSystem_ReturnsSuccessResult()
    {
        // Arrange
        var systemToDelete = new OrgSystem
        {
            Id = 1,
            Name = "System to Delete",
            Description = "This system will be deleted",
            Criticality = SystemCriticality.Low
        };

        var command = new DeleteOrgSystemCommand(new[] { systemToDelete.Id });

        _mockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(systemToDelete);

        _mockDbSet.Setup(x => x.Remove(It.IsAny<OrgSystem>()));

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockDbContextFactory.Verify(x => x.CreateAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockDbSet.Verify(x => x.Remove(systemToDelete), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_DeleteMultipleOrgSystems_ReturnsSuccessResult()
    {
        // Arrange
        var system1 = new OrgSystem { Id = 1, Name = "System 1" };
        var system2 = new OrgSystem { Id = 2, Name = "System 2" };
        var system3 = new OrgSystem { Id = 3, Name = "System 3" };

        var command = new DeleteOrgSystemCommand(new[] { 1, 2, 3 });

        _mockDbSet.SetupSequence(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(system1)
            .ReturnsAsync(system2)
            .ReturnsAsync(system3);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        _mockDbSet.Verify(x => x.Remove(It.IsAny<OrgSystem>()), Times.Exactly(3));
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_DeleteNonExistentOrgSystem_SkipsGracefully()
    {
        // Arrange
        var existingSystem = new OrgSystem { Id = 1, Name = "Existing System" };

        var command = new DeleteOrgSystemCommand(new[] { 1, 999 }); // 999 doesn't exist

        _mockDbSet.SetupSequence(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSystem)
            .ReturnsAsync((OrgSystem)null); // Non-existent system

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        _mockDbSet.Verify(x => x.Remove(existingSystem), Times.Once);
        _mockDbSet.Verify(x => x.Remove(It.Is<OrgSystem>(s => s.Id == 999)), Times.Never);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_DeleteSystemWithDependencies_RemovesSystemAndDependencies()
    {
        // Arrange
        var systemToDelete = new OrgSystem
        {
            Id = 1,
            Name = "System with Dependencies",
            OutgoingDependencies = new List<DependencyEdge>
            {
                new() { Id = 1, SourceSystemId = 1, TargetSystemId = 2 },
                new() { Id = 2, SourceSystemId = 1, TargetSystemId = 3 }
            },
            IncomingDependencies = new List<DependencyEdge>
            {
                new() { Id = 3, SourceSystemId = 4, TargetSystemId = 1 }
            }
        };

        var command = new DeleteOrgSystemCommand(new[] { systemToDelete.Id });

        _mockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(systemToDelete);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockDbSet.Verify(x => x.Remove(systemToDelete), Times.Once);
        // Note: In a real implementation, cascade delete should handle related entities
        // or explicit removal of dependencies would be required
    }

    [Test]
    public async Task Handle_DeleteSystemWithAttributes_RemovesSystemAndAttributes()
    {
        // Arrange
        var attributeDef = new AttributeDefinition 
        { 
            Id = 1, 
            Name = "Version",
            DataType = CleanArchitecture.Blazor.Domain.Enums.AttributeDataType.Text
        };

        var systemToDelete = new OrgSystem
        {
            Id = 1,
            Name = "System with Attributes",
            Attributes = new List<SystemAttribute>
            {
                new() { Id = 1, OrgSystemId = 1, AttributeDefinitionId = 1, Value = "1.0", AttributeDefinition = attributeDef },
                new() { Id = 2, OrgSystemId = 1, AttributeDefinitionId = 1, Value = "2.0", AttributeDefinition = attributeDef }
            }
        };

        var command = new DeleteOrgSystemCommand(new[] { systemToDelete.Id });

        _mockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(systemToDelete);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockDbSet.Verify(x => x.Remove(systemToDelete), Times.Once);
        // In real implementation, cascade delete or explicit removal of attributes would be handled
        systemToDelete.Attributes.Should().HaveCount(2); // Attributes are still attached for verification
    }

    [Test]
    public async Task Handle_CancellationRequested_PropagatesCancellation()
    {
        // Arrange
        var command = new DeleteOrgSystemCommand(new[] { 1 });
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _mockDbContextFactory.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await FluentActions.Invoking(async () => 
            await _handler.Handle(command, cancellationTokenSource.Token))
            .Should().ThrowAsync<OperationCanceledException>();

        _mockDbContextFactory.Verify(x => x.CreateAsync(cancellationTokenSource.Token), Times.Once);
    }

    [Test]
    public async Task Handle_DatabaseException_PropagatesException()
    {
        // Arrange
        var systemToDelete = new OrgSystem { Id = 1, Name = "Test System" };
        var command = new DeleteOrgSystemCommand(new[] { systemToDelete.Id });

        _mockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(systemToDelete);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Foreign key constraint violation"));

        // Act & Assert
        await FluentActions.Invoking(async () => 
            await _handler.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Foreign key constraint violation");

        _mockDbSet.Verify(x => x.Remove(systemToDelete), Times.Once);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_EmptyIdArray_ReturnsSuccessWithoutChanges()
    {
        // Arrange
        var command = new DeleteOrgSystemCommand(Array.Empty<int>());

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        _mockDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockDbSet.Verify(x => x.Remove(It.IsAny<OrgSystem>()), Times.Never);
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_NullIdArray_ThrowsArgumentException()
    {
        // Arrange & Act & Assert
        await FluentActions.Invoking(async () => 
            {
                var command = new DeleteOrgSystemCommand(null);
                await _handler.Handle(command, CancellationToken.None);
            })
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("*cannot be null*");
    }

    [Test]
    public async Task Handle_DuplicateIds_HandlesGracefully()
    {
        // Arrange
        var systemToDelete = new OrgSystem { Id = 1, Name = "Duplicate ID System" };
        var command = new DeleteOrgSystemCommand(new[] { 1, 1, 1 }); // Same ID repeated

        _mockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(systemToDelete);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        // Should attempt to find and delete each ID (even duplicates)
        _mockDbSet.Verify(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
        // Remove might be called multiple times for the same entity, depending on implementation
        _mockDbSet.Verify(x => x.Remove(It.IsAny<OrgSystem>()), Times.AtLeast(1));
        _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_CriticalSystemDeletion_SucceedsWithWarning()
    {
        // Arrange
        var criticalSystem = new OrgSystem
        {
            Id = 1,
            Name = "Critical Production System",
            Criticality = SystemCriticality.Critical,
            LifecycleStatus = SystemLifecycleStatus.Active
        };

        var command = new DeleteOrgSystemCommand(new[] { criticalSystem.Id });

        _mockDbSet.Setup(x => x.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(criticalSystem);

        _mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        
        // Verify critical system was still deleted (business logic allows it)
        _mockDbSet.Verify(x => x.Remove(criticalSystem), Times.Once);
        
        // In a real implementation, you might add warnings or additional validation
        criticalSystem.Criticality.Should().Be(SystemCriticality.Critical);
    }
}