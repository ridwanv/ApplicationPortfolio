// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.DTOs;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Queries.Pagination;
using CleanArchitecture.Blazor.Domain.Common.Enums;
using CleanArchitecture.Blazor.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.UnitTests.Features.OrgSystems.Queries;

[TestFixture]
public class OrgSystemsPaginationQueryHandlerTests
{
    private Mock<IApplicationDbContextFactory> _mockDbContextFactory;
    private Mock<IApplicationDbContext> _mockDbContext;
    private Mock<IMapper> _mockMapper;
    private OrgSystemsPaginationQueryHandler _handler;
    private List<OrgSystem> _testOrgSystems;
    private List<OrgSystemDto> _testOrgSystemDtos;

    [SetUp]
    public void SetUp()
    {
        _mockDbContextFactory = new Mock<IApplicationDbContextFactory>();
        _mockDbContext = new Mock<IApplicationDbContext>();
        _mockMapper = new Mock<IMapper>();

        _testOrgSystems = CreateTestOrgSystems();
        _testOrgSystemDtos = CreateTestOrgSystemDtos();

        var mockDbSet = _testOrgSystems.AsQueryable().BuildMockDbSet();
        _mockDbContext.Setup(x => x.OrgSystems).Returns(mockDbSet.Object);
        
        _mockDbContextFactory.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_mockDbContext.Object);

        _handler = new OrgSystemsPaginationQueryHandler(_mockDbContextFactory.Object, _mockMapper.Object);
    }

    [Test]
    public async Task Handle_GetFirstPage_ReturnsCorrectPaginatedResult()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 2,
            OrderBy = "Name"
        };

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(_testOrgSystemDtos.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalItems.Should().Be(_testOrgSystems.Count);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(2);
        result.TotalPages.Should().Be((int)Math.Ceiling((double)_testOrgSystems.Count / 2));

        _mockDbContextFactory.Verify(x => x.CreateAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockMapper.Verify(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()), Times.Once);
    }

    [Test]
    public async Task Handle_GetSecondPage_ReturnsCorrectPaginatedResult()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 2,
            PageSize = 3,
            OrderBy = "Name"
        };

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(_testOrgSystemDtos.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(3);
        result.TotalItems.Should().Be(_testOrgSystems.Count);

        _mockMapper.Verify(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()), Times.Once);
    }

    [Test]
    public async Task Handle_WithKeywordFilter_ReturnsFilteredResults()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            Keyword = "HR", // Should match "HR Management System"
            OrderBy = "Name"
        };

        var filteredSystems = _testOrgSystems
            .Where(s => s.Name.Contains("HR", StringComparison.OrdinalIgnoreCase) ||
                       (s.Description != null && s.Description.Contains("HR", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var filteredDtos = _testOrgSystemDtos
            .Where(s => s.Name.Contains("HR", StringComparison.OrdinalIgnoreCase) ||
                       (s.Description != null && s.Description.Contains("HR", StringComparison.OrdinalIgnoreCase)))
            .AsQueryable();

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(filteredDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().Name.Should().Contain("HR");
        result.TotalItems.Should().Be(1);
    }

    [Test]
    public async Task Handle_WithCriticalityFilter_ReturnsFilteredResults()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            AdvancedFilter = new()
            {
                Criticality = SystemCriticality.Critical
            },
            OrderBy = "Name"
        };

        var filteredSystems = _testOrgSystems
            .Where(s => s.Criticality == SystemCriticality.Critical)
            .ToList();

        var filteredDtos = _testOrgSystemDtos
            .Where(s => s.Criticality == SystemCriticality.Critical)
            .AsQueryable();

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(filteredDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().Criticality.Should().Be(SystemCriticality.Critical);
    }

    [Test]
    public async Task Handle_WithLifecycleStatusFilter_ReturnsFilteredResults()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            AdvancedFilter = new()
            {
                LifecycleStatus = SystemLifecycleStatus.Retired
            },
            OrderBy = "Name"
        };

        var filteredDtos = _testOrgSystemDtos
            .Where(s => s.LifecycleStatus == SystemLifecycleStatus.Retired)
            .AsQueryable();

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(filteredDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data.First().LifecycleStatus.Should().Be(SystemLifecycleStatus.Retired);
    }

    [Test]
    public async Task Handle_WithDateRangeFilter_ReturnsFilteredResults()
    {
        // Arrange
        var filterDate = DateTime.UtcNow.AddDays(-30);
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            AdvancedFilter = new()
            {
                CreatedAfter = filterDate
            },
            OrderBy = "Created"
        };

        var filteredDtos = _testOrgSystemDtos
            .Where(s => s.Created >= filterDate)
            .AsQueryable();

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(filteredDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().OnlyContain(s => s.Created >= filterDate);
    }

    [Test]
    public async Task Handle_WithOwnerIdFilter_ReturnsFilteredResults()
    {
        // Arrange
        var ownerId = 1;
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            AdvancedFilter = new()
            {
                BusinessOwnerId = ownerId
            },
            OrderBy = "Name"
        };

        var filteredDtos = _testOrgSystemDtos
            .Where(s => s.BusinessOwnerId == ownerId)
            .AsQueryable();

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(filteredDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().OnlyContain(s => s.BusinessOwnerId == ownerId);
    }

    [Test]
    public async Task Handle_WithComplexFilter_ReturnsFilteredResults()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            Keyword = "System",
            AdvancedFilter = new()
            {
                Criticality = SystemCriticality.High,
                LifecycleStatus = SystemLifecycleStatus.Active,
                BusinessOwnerId = 1
            },
            OrderBy = "Name"
        };

        var filteredDtos = _testOrgSystemDtos
            .Where(s => (s.Name.Contains("System", StringComparison.OrdinalIgnoreCase) ||
                        (s.Description != null && s.Description.Contains("System", StringComparison.OrdinalIgnoreCase))) &&
                       s.Criticality == SystemCriticality.High &&
                       s.LifecycleStatus == SystemLifecycleStatus.Active &&
                       s.BusinessOwnerId == 1)
            .AsQueryable();

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(filteredDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().OnlyContain(s => 
            s.Criticality == SystemCriticality.High &&
            s.LifecycleStatus == SystemLifecycleStatus.Active &&
            s.BusinessOwnerId == 1);
    }

    [Test]
    public async Task Handle_OrderByName_ReturnsCorrectlyOrderedResults()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "Name"
        };

        var orderedDtos = _testOrgSystemDtos.OrderBy(s => s.Name).AsQueryable();

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(orderedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeInAscendingOrder(s => s.Name);
    }

    [Test]
    public async Task Handle_OrderByCreatedDescending_ReturnsCorrectlyOrderedResults()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "Created desc"
        };

        var orderedDtos = _testOrgSystemDtos.OrderByDescending(s => s.Created).AsQueryable();

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(orderedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeInDescendingOrder(s => s.Created);
    }

    [Test]
    public async Task Handle_EmptyResults_ReturnsEmptyPaginatedData()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10,
            Keyword = "NonExistentSystem"
        };

        var emptyResults = new List<OrgSystemDto>().AsQueryable();

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(emptyResults);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Test]
    public async Task Handle_CancellationRequested_PropagatesCancellation()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery { PageNumber = 1, PageSize = 10 };
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        _mockDbContextFactory.Setup(x => x.CreateAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new OperationCanceledException());

        // Act & Assert
        await FluentActions.Invoking(async () => 
            await _handler.Handle(query, cancellationTokenSource.Token))
            .Should().ThrowAsync<OperationCanceledException>();
    }

    [Test]
    public async Task Handle_InvalidPageNumber_ReturnsEmptyResults()
    {
        // Arrange
        var query = new OrgSystemsPaginationQuery
        {
            PageNumber = 100, // Way beyond available pages
            PageSize = 10,
            OrderBy = "Name"
        };

        _mockMapper.Setup(x => x.ProjectTo<OrgSystemDto>(It.IsAny<IQueryable<OrgSystem>>()))
            .Returns(_testOrgSystemDtos.AsQueryable());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.PageNumber.Should().Be(100);
        result.TotalItems.Should().Be(_testOrgSystems.Count);
    }

    private List<OrgSystem> CreateTestOrgSystems()
    {
        return new List<OrgSystem>
        {
            new()
            {
                Id = 1,
                Name = "HR Management System",
                Description = "Human Resources Management System",
                BusinessOwnerId = 1,
                TechnicalOwnerId = 2,
                Criticality = SystemCriticality.High,
                LifecycleStatus = SystemLifecycleStatus.Active,
                Tags = new List<string> { "HR", "Management" },
                Created = DateTime.UtcNow.AddDays(-10)
            },
            new()
            {
                Id = 2,
                Name = "Financial Reporting System",
                Description = "Financial data processing and reporting",
                BusinessOwnerId = 2,
                TechnicalOwnerId = 1,
                Criticality = SystemCriticality.Critical,
                LifecycleStatus = SystemLifecycleStatus.Active,
                Tags = new List<string> { "Finance", "Reporting" },
                Created = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = 3,
                Name = "Legacy Inventory System",
                Description = "Old inventory management system",
                BusinessOwnerId = 3,
                TechnicalOwnerId = 3,
                Criticality = SystemCriticality.Low,
                LifecycleStatus = SystemLifecycleStatus.Retired,
                Tags = new List<string> { "Legacy", "Inventory" },
                Created = DateTime.UtcNow.AddDays(-100)
            },
            new()
            {
                Id = 4,
                Name = "Customer Portal",
                Description = "External customer facing portal",
                BusinessOwnerId = 1,
                TechnicalOwnerId = 2,
                Criticality = SystemCriticality.Medium,
                LifecycleStatus = SystemLifecycleStatus.Active,
                Tags = new List<string> { "Customer", "Portal", "External" },
                Created = DateTime.UtcNow.AddDays(-20)
            },
            new()
            {
                Id = 5,
                Name = "Analytics Platform",
                Description = "Business intelligence and analytics platform",
                BusinessOwnerId = 2,
                TechnicalOwnerId = 1,
                Criticality = SystemCriticality.High,
                LifecycleStatus = SystemLifecycleStatus.Active,
                Tags = new List<string> { "Analytics", "BI", "Intelligence" },
                Created = DateTime.UtcNow.AddDays(-2)
            }
        };
    }

    private List<OrgSystemDto> CreateTestOrgSystemDtos()
    {
        return _testOrgSystems.Select(s => new OrgSystemDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            BusinessOwnerId = s.BusinessOwnerId,
            TechnicalOwnerId = s.TechnicalOwnerId,
            Criticality = s.Criticality,
            LifecycleStatus = s.LifecycleStatus,
            Tags = s.Tags,
            Dependencies = s.Dependencies,
            Created = s.Created
        }).ToList();
    }
}