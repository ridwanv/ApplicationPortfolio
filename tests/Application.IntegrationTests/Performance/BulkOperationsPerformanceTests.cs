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
using System.Diagnostics;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Performance;

using static Testing;

[TestFixture]
public class BulkOperationsPerformanceTests : TestBase
{
    private const int SMALL_BATCH_SIZE = 10;
    private const int MEDIUM_BATCH_SIZE = 100;
    private const int LARGE_BATCH_SIZE = 500;
    private const int PERFORMANCE_THRESHOLD_MS = 5000; // 5 seconds
    private const int QUERY_PERFORMANCE_THRESHOLD_MS = 1000; // 1 second

    [Test]
    public async Task BulkCreateOrgSystems_SmallBatch_CompletesWithinPerformanceThreshold()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var systemIds = new List<int>();
        for (int i = 1; i <= SMALL_BATCH_SIZE; i++)
        {
            var command = new AddEditOrgSystemCommand
            {
                Name = $"Bulk System {i}",
                Description = $"System created during bulk operation {i}",
                Criticality = GetRandomCriticality(),
                LifecycleStatus = SystemLifecycleStatus.Active,
                Tags = new List<string> { "Bulk", "Performance", $"Batch{i}" }
            };

            var result = await SendAsync(command);
            result.Succeeded.Should().BeTrue();
            systemIds.Add(result.Data);
        }

        stopwatch.Stop();

        // Assert
        systemIds.Should().HaveCount(SMALL_BATCH_SIZE);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(PERFORMANCE_THRESHOLD_MS, 
            $"Creating {SMALL_BATCH_SIZE} systems should complete within {PERFORMANCE_THRESHOLD_MS}ms");

        Console.WriteLine($"Created {SMALL_BATCH_SIZE} systems in {stopwatch.ElapsedMilliseconds}ms");

        // Verify systems were created
        var createdSystems = await CountAsync<OrgSystem>();
        createdSystems.Should().BeGreaterOrEqualTo(SMALL_BATCH_SIZE);
    }

    [Test]
    public async Task BulkCreateOrgSystems_MediumBatch_CompletesWithinPerformanceThreshold()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var systemIds = new List<int>();
        for (int i = 1; i <= MEDIUM_BATCH_SIZE; i++)
        {
            var command = new AddEditOrgSystemCommand
            {
                Name = $"Medium Bulk System {i}",
                Description = $"System created during medium bulk operation {i}",
                BusinessOwnerId = (i % 5) + 1, // Distribute across 5 owners
                TechnicalOwnerId = (i % 3) + 1, // Distribute across 3 technical owners
                Criticality = GetRandomCriticality(),
                LifecycleStatus = SystemLifecycleStatus.Active,
                Tags = new List<string> { "MediumBulk", "Performance", $"Batch{i / 10}" }
            };

            var result = await SendAsync(command);
            result.Succeeded.Should().BeTrue();
            systemIds.Add(result.Data);
        }

        stopwatch.Stop();

        // Assert
        systemIds.Should().HaveCount(MEDIUM_BATCH_SIZE);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(PERFORMANCE_THRESHOLD_MS * 2, 
            $"Creating {MEDIUM_BATCH_SIZE} systems should complete within {PERFORMANCE_THRESHOLD_MS * 2}ms");

        Console.WriteLine($"Created {MEDIUM_BATCH_SIZE} systems in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Average time per system: {(double)stopwatch.ElapsedMilliseconds / MEDIUM_BATCH_SIZE:F2}ms");

        // Verify systems were created
        var createdSystems = await CountAsync<OrgSystem>();
        createdSystems.Should().BeGreaterOrEqualTo(MEDIUM_BATCH_SIZE);
    }

    [Test]
    [Ignore("Long running test - run manually for performance validation")]
    public async Task BulkCreateOrgSystems_LargeBatch_CompletesWithinExtendedThreshold()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();

        // Act
        var systemIds = new List<int>();
        var batchSize = 50; // Process in smaller batches for better performance

        for (int batch = 0; batch < LARGE_BATCH_SIZE / batchSize; batch++)
        {
            var batchTasks = new List<Task<CleanArchitecture.Blazor.Application.Common.Models.Result<int>>>();
            
            for (int i = 1; i <= batchSize; i++)
            {
                var systemNumber = batch * batchSize + i;
                var command = new AddEditOrgSystemCommand
                {
                    Name = $"Large Bulk System {systemNumber}",
                    Description = $"System created during large bulk operation {systemNumber}",
                    BusinessOwnerId = (systemNumber % 10) + 1,
                    TechnicalOwnerId = (systemNumber % 5) + 1,
                    Criticality = GetRandomCriticality(),
                    LifecycleStatus = SystemLifecycleStatus.Active,
                    Tags = new List<string> { "LargeBulk", "Performance", $"Batch{batch}" }
                };

                batchTasks.Add(SendAsync(command));
            }

            var batchResults = await Task.WhenAll(batchTasks);
            foreach (var result in batchResults)
            {
                result.Succeeded.Should().BeTrue();
                systemIds.Add(result.Data);
            }

            Console.WriteLine($"Completed batch {batch + 1}/{LARGE_BATCH_SIZE / batchSize}");
        }

        stopwatch.Stop();

        // Assert
        systemIds.Should().HaveCount(LARGE_BATCH_SIZE);
        Console.WriteLine($"Created {LARGE_BATCH_SIZE} systems in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Average time per system: {(double)stopwatch.ElapsedMilliseconds / LARGE_BATCH_SIZE:F2}ms");

        // Performance threshold is more lenient for large batches
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(PERFORMANCE_THRESHOLD_MS * 10, 
            $"Creating {LARGE_BATCH_SIZE} systems should complete within reasonable time");
    }

    [Test]
    public async Task BulkAttributeBackfill_MediumBatch_CompletesWithinPerformanceThreshold()
    {
        // Arrange - First create systems
        var systemIds = new List<int>();
        for (int i = 1; i <= MEDIUM_BATCH_SIZE; i++)
        {
            var command = new AddEditOrgSystemCommand
            {
                Name = $"Attribute Test System {i}",
                Description = $"System for attribute backfill testing {i}",
                Criticality = SystemCriticality.Medium,
                LifecycleStatus = SystemLifecycleStatus.Active
            };

            var result = await SendAsync(command);
            systemIds.Add(result.Data);
        }

        // Create attribute definition
        var attributeDefCommand = new AddEditAttributeDefinitionCommand
        {
            Name = "BulkAttribute",
            Description = "Attribute for bulk testing",
            DataType = AttributeDataType.Text,
            IsRequired = false
        };

        var attributeDefResult = await SendAsync(attributeDefCommand);
        var attributeDefId = attributeDefResult.Data;

        var stopwatch = Stopwatch.StartNew();

        // Act - Backfill attribute to all systems
        var context = CreateDbContext();
        var attributes = new List<SystemAttribute>();

        for (int i = 0; i < systemIds.Count; i++)
        {
            attributes.Add(new SystemAttribute
            {
                OrgSystemId = systemIds[i],
                AttributeDefinitionId = attributeDefId,
                Value = $"BulkValue{i + 1}"
            });
        }

        context.SystemAttributes.AddRange(attributes);
        await context.SaveChangesAsync();

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(PERFORMANCE_THRESHOLD_MS, 
            $"Backfilling attributes to {MEDIUM_BATCH_SIZE} systems should complete within {PERFORMANCE_THRESHOLD_MS}ms");

        Console.WriteLine($"Backfilled attributes to {MEDIUM_BATCH_SIZE} systems in {stopwatch.ElapsedMilliseconds}ms");

        // Verify attributes were added
        var attributeCount = await context.SystemAttributes
            .Where(a => a.AttributeDefinitionId == attributeDefId)
            .CountAsync();
        
        attributeCount.Should().Be(MEDIUM_BATCH_SIZE);
    }

    [Test]
    public async Task BulkDependencyCreation_NetworkTopology_CompletesWithinPerformanceThreshold()
    {
        // Arrange - Create a network of systems
        var coreSystemIds = new List<int>();
        var edgeSystemIds = new List<int>();

        // Create 10 core systems
        for (int i = 1; i <= 10; i++)
        {
            var command = new AddEditOrgSystemCommand
            {
                Name = $"Core System {i}",
                Description = $"Core system for dependency network {i}",
                Criticality = SystemCriticality.Critical,
                LifecycleStatus = SystemLifecycleStatus.Active
            };

            var result = await SendAsync(command);
            coreSystemIds.Add(result.Data);
        }

        // Create 40 edge systems
        for (int i = 1; i <= 40; i++)
        {
            var command = new AddEditOrgSystemCommand
            {
                Name = $"Edge System {i}",
                Description = $"Edge system for dependency network {i}",
                Criticality = SystemCriticality.Medium,
                LifecycleStatus = SystemLifecycleStatus.Active
            };

            var result = await SendAsync(command);
            edgeSystemIds.Add(result.Data);
        }

        var stopwatch = Stopwatch.StartNew();

        // Act - Create dependency network: each edge system depends on 2-3 core systems
        var context = CreateDbContext();
        var dependencies = new List<DependencyEdge>();

        foreach (var edgeSystemId in edgeSystemIds)
        {
            // Each edge system depends on 2-3 randomly selected core systems
            var dependencyCount = new Random().Next(2, 4);
            var selectedCoreSystemIds = coreSystemIds.OrderBy(x => Guid.NewGuid()).Take(dependencyCount);

            foreach (var coreSystemId in selectedCoreSystemIds)
            {
                dependencies.Add(new DependencyEdge
                {
                    SourceSystemId = edgeSystemId,
                    TargetSystemId = coreSystemId,
                    Notes = $"Dependency from Edge System to Core System {coreSystemId}"
                });
            }
        }

        context.DependencyEdges.AddRange(dependencies);
        await context.SaveChangesAsync();

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(PERFORMANCE_THRESHOLD_MS, 
            $"Creating dependency network should complete within {PERFORMANCE_THRESHOLD_MS}ms");

        Console.WriteLine($"Created {dependencies.Count} dependencies in {stopwatch.ElapsedMilliseconds}ms");

        // Verify dependencies were created
        var dependencyCount = await context.DependencyEdges.CountAsync();
        dependencyCount.Should().BeGreaterOrEqualTo(dependencies.Count);
    }

    [Test]
    public async Task BulkDelete_MediumBatch_CompletesWithinPerformanceThreshold()
    {
        // Arrange - Create systems to delete
        var systemIds = new List<int>();
        for (int i = 1; i <= MEDIUM_BATCH_SIZE; i++)
        {
            var command = new AddEditOrgSystemCommand
            {
                Name = $"Delete Test System {i}",
                Description = $"System to be deleted {i}",
                Criticality = SystemCriticality.Low,
                LifecycleStatus = SystemLifecycleStatus.Inactive
            };

            var result = await SendAsync(command);
            systemIds.Add(result.Data);
        }

        var stopwatch = Stopwatch.StartNew();

        // Act - Delete systems in batches (delete command typically handles batch operations)
        const int deleteBatchSize = 20; // Delete in smaller batches to avoid timeout
        var deletedCount = 0;

        for (int i = 0; i < systemIds.Count; i += deleteBatchSize)
        {
            var batchToDelete = systemIds.Skip(i).Take(deleteBatchSize).ToArray();
            var deleteCommand = new DeleteOrgSystemCommand(batchToDelete);
            
            var result = await SendAsync(deleteCommand);
            result.Succeeded.Should().BeTrue();
            deletedCount += batchToDelete.Length;
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(PERFORMANCE_THRESHOLD_MS, 
            $"Deleting {MEDIUM_BATCH_SIZE} systems should complete within {PERFORMANCE_THRESHOLD_MS}ms");

        Console.WriteLine($"Deleted {deletedCount} systems in {stopwatch.ElapsedMilliseconds}ms");

        // Verify systems were deleted
        var remainingSystems = CreateDbContext().OrgSystems.Where(s => systemIds.Contains(s.Id)).ToList();
        remainingSystems.Should().BeEmpty();
    }

    [Test]
    public async Task BulkQuery_LargeDataset_CompletesWithinPerformanceThreshold()
    {
        // Arrange - Create dataset to query
        var systemIds = new List<int>();
        for (int i = 1; i <= MEDIUM_BATCH_SIZE; i++)
        {
            var command = new AddEditOrgSystemCommand
            {
                Name = $"Query Test System {i}",
                Description = $"System for query performance testing {i}",
                BusinessOwnerId = (i % 10) + 1,
                Criticality = GetRandomCriticality(),
                LifecycleStatus = GetRandomLifecycleStatus(),
                Tags = new List<string> { "QueryTest", $"Group{i % 5}", "Performance" }
            };

            var result = await SendAsync(command);
            systemIds.Add(result.Data);
        }

        var stopwatch = Stopwatch.StartNew();

        // Act - Perform various queries
        var queries = new List<Task<CleanArchitecture.Blazor.Application.Common.Models.PaginatedData<CleanArchitecture.Blazor.Application.Features.OrgSystems.DTOs.OrgSystemDto>>>();

        // Query 1: Get all systems
        queries.Add(SendAsync(new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 50,
            OrderBy = "Name"
        }));

        // Query 2: Filter by criticality
        queries.Add(SendAsync(new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 50,
            AdvancedFilter = new() { Criticality = SystemCriticality.Critical }
        }));

        // Query 3: Filter by lifecycle status
        queries.Add(SendAsync(new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 50,
            AdvancedFilter = new() { LifecycleStatus = SystemLifecycleStatus.Active }
        }));

        // Query 4: Search by keyword
        queries.Add(SendAsync(new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 50,
            Keyword = "QueryTest"
        }));

        // Query 5: Complex filter
        queries.Add(SendAsync(new OrgSystemsPaginationQuery
        {
            PageNumber = 1,
            PageSize = 50,
            Keyword = "System",
            AdvancedFilter = new() 
            { 
                Criticality = SystemCriticality.High,
                LifecycleStatus = SystemLifecycleStatus.Active
            }
        }));

        var results = await Task.WhenAll(queries);

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(QUERY_PERFORMANCE_THRESHOLD_MS * 5, 
            "Multiple complex queries should complete within performance threshold");

        Console.WriteLine($"Executed {queries.Count} queries against {MEDIUM_BATCH_SIZE} systems in {stopwatch.ElapsedMilliseconds}ms");

        results.Should().OnlyContain(r => r != null);
        results.Should().OnlyContain(r => r.Data != null);
    }

    [Test]
    public async Task BulkComplexOperations_MixedWorkload_CompletesWithinPerformanceThreshold()
    {
        // Arrange
        var stopwatch = Stopwatch.StartNew();
        var operationResults = new List<bool>();

        // Act - Mixed workload simulation
        
        // Phase 1: Create systems (30% of operations)
        for (int i = 1; i <= 15; i++)
        {
            var command = new AddEditOrgSystemCommand
            {
                Name = $"Mixed Workload System {i}",
                Description = $"System for mixed workload testing {i}",
                Criticality = GetRandomCriticality(),
                LifecycleStatus = SystemLifecycleStatus.Active
            };

            var result = await SendAsync(command);
            operationResults.Add(result.Succeeded);
        }

        // Phase 2: Query operations (40% of operations)
        for (int i = 1; i <= 20; i++)
        {
            var query = new OrgSystemsPaginationQuery
            {
                PageNumber = 1,
                PageSize = 10,
                OrderBy = i % 2 == 0 ? "Name" : "Created desc"
            };

            var result = await SendAsync(query);
            operationResults.Add(result != null);
        }

        // Phase 3: Update operations (20% of operations)
        var existingSystems = CreateDbContext().OrgSystems.Take(10).ToList();
        foreach (var system in existingSystems)
        {
            var updateCommand = new AddEditOrgSystemCommand
            {
                Id = system.Id,
                Name = system.Name + " - Updated",
                Description = system.Description,
                Criticality = system.Criticality,
                LifecycleStatus = system.LifecycleStatus
            };

            var result = await SendAsync(updateCommand);
            operationResults.Add(result.Succeeded);
        }

        // Phase 4: Create attributes and relationships (10% of operations)
        var context = CreateDbContext();
        var attributeDef = new AttributeDefinition
        {
            Name = "MixedWorkloadAttribute",
            DataType = AttributeDataType.Text,
            IsRequired = false
        };
        
        context.AttributeDefinitions.Add(attributeDef);
        await context.SaveChangesAsync();

        foreach (var system in existingSystems.Take(5))
        {
            var attr = new SystemAttribute
            {
                OrgSystemId = system.Id,
                AttributeDefinitionId = attributeDef.Id,
                Value = $"MixedValue-{system.Id}"
            };
            
            context.SystemAttributes.Add(attr);
        }
        
        await context.SaveChangesAsync();
        operationResults.Add(true);

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(PERFORMANCE_THRESHOLD_MS * 2, 
            "Mixed workload operations should complete within extended performance threshold");

        Console.WriteLine($"Executed mixed workload of {operationResults.Count} operations in {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Average time per operation: {(double)stopwatch.ElapsedMilliseconds / operationResults.Count:F2}ms");

        operationResults.Should().OnlyContain(r => r == true, "All operations should succeed");
    }

    // Helper methods
    private SystemCriticality GetRandomCriticality()
    {
        var values = Enum.GetValues<SystemCriticality>();
        return values[new Random().Next(values.Length)];
    }

    private SystemLifecycleStatus GetRandomLifecycleStatus()
    {
        var values = Enum.GetValues<SystemLifecycleStatus>();
        return values[new Random().Next(values.Length)];
    }
}