// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.SystemDependencies.Caching;
using CleanArchitecture.Blazor.Application.Features.SystemDependencies.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.SystemDependencies.Queries.GetDependencyGraph;

public class GetDependencyGraphQuery : ICacheableRequest<Result<List<SystemDependencyNodeDto>>>
{
    public int? RootSystemId { get; set; }
    public int MaxDepth { get; set; } = 5;
    public bool IncludeOrphans { get; set; } = true;
    
    public string CacheKey => SystemDependencyCacheKey.GetDependencyGraphCacheKey(RootSystemId);
    public IEnumerable<string>? Tags => SystemDependencyCacheKey.Tags;
}

public class GetDependencyGraphQueryHandler : IRequestHandler<GetDependencyGraphQuery, Result<List<SystemDependencyNodeDto>>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public GetDependencyGraphQueryHandler(IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result<List<SystemDependencyNodeDto>>> Handle(GetDependencyGraphQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        // Get all systems and dependencies
        var systems = await db.OrgSystems
            .AsNoTracking()
            .Select(s => new SystemDependencyNodeDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Criticality = s.Criticality,
                LifecycleStatus = s.LifecycleStatus
            })
            .ToListAsync(cancellationToken);

        var systemsDict = systems.ToDictionary(s => s.Id);

        var dependencies = await db.DependencyEdges
            .AsNoTracking()
            .Include(d => d.SourceSystem)
            .Include(d => d.TargetSystem)
            .Select(d => new
            {
                DependencyId = d.Id,
                SourceId = d.SourceSystemId,
                SourceName = d.SourceSystem.Name,
                TargetId = d.TargetSystemId,
                TargetName = d.TargetSystem.Name,
                Notes = d.Notes
            })
            .ToListAsync(cancellationToken);

        // Build dependency connections
        foreach (var dep in dependencies)
        {
            if (systemsDict.TryGetValue(dep.SourceId, out var sourceSystem))
            {
                sourceSystem.Dependencies.Add(new DependencyConnectionDto
                {
                    DependencyId = dep.DependencyId,
                    SystemId = dep.TargetId,
                    SystemName = dep.TargetName,
                    Notes = dep.Notes
                });
            }

            if (systemsDict.TryGetValue(dep.TargetId, out var targetSystem))
            {
                targetSystem.Dependents.Add(new DependencyConnectionDto
                {
                    DependencyId = dep.DependencyId,
                    SystemId = dep.SourceId,
                    SystemName = dep.SourceName,
                    Notes = dep.Notes
                });
            }
        }

        // Filter based on root system if specified
        List<SystemDependencyNodeDto> result;
        if (request.RootSystemId.HasValue)
        {
            if (systemsDict.TryGetValue(request.RootSystemId.Value, out var rootSystem))
            {
                var connectedSystems = GetConnectedSystems(rootSystem, systemsDict, request.MaxDepth);
                result = connectedSystems.ToList();
            }
            else
            {
                return await Result<List<SystemDependencyNodeDto>>.FailureAsync($"Root system with id: [{request.RootSystemId}] not found.");
            }
        }
        else
        {
            result = systems.Where(s => request.IncludeOrphans || !s.IsOrphan).ToList();
        }

        // Calculate levels for visualization
        CalculateSystemLevels(result);

        return Result<List<SystemDependencyNodeDto>>.Success(result);
    }

    private HashSet<SystemDependencyNodeDto> GetConnectedSystems(SystemDependencyNodeDto root, Dictionary<int, SystemDependencyNodeDto> systemsDict, int maxDepth)
    {
        var connected = new HashSet<SystemDependencyNodeDto>();
        var queue = new Queue<(SystemDependencyNodeDto system, int depth)>();
        queue.Enqueue((root, 0));
        
        while (queue.Count > 0)
        {
            var (current, depth) = queue.Dequeue();
            if (connected.Contains(current) || depth > maxDepth) continue;
            
            connected.Add(current);
            
            // Add dependencies and dependents
            foreach (var dep in current.Dependencies.Concat(current.Dependents))
            {
                if (systemsDict.TryGetValue(dep.SystemId, out var relatedSystem) && !connected.Contains(relatedSystem))
                {
                    queue.Enqueue((relatedSystem, depth + 1));
                }
            }
        }
        
        return connected;
    }

    private void CalculateSystemLevels(List<SystemDependencyNodeDto> systems)
    {
        var systemsDict = systems.ToDictionary(s => s.Id);
        var visited = new HashSet<int>();
        
        // Start with systems that have no dependencies (root systems)
        var rootSystems = systems.Where(s => !s.Dependencies.Any()).ToList();
        
        foreach (var root in rootSystems)
        {
            CalculateLevelRecursive(root, systemsDict, visited, 0);
        }
        
        // Handle any remaining systems (might be part of cycles)
        foreach (var system in systems.Where(s => !visited.Contains(s.Id)))
        {
            CalculateLevelRecursive(system, systemsDict, visited, 0);
        }
    }

    private void CalculateLevelRecursive(SystemDependencyNodeDto system, Dictionary<int, SystemDependencyNodeDto> systemsDict, HashSet<int> visited, int level)
    {
        if (visited.Contains(system.Id)) return;
        
        visited.Add(system.Id);
        system.Level = Math.Max(system.Level, level);
        
        // Process dependents (systems that depend on this one)
        foreach (var dependent in system.Dependents)
        {
            if (systemsDict.TryGetValue(dependent.SystemId, out var dependentSystem))
            {
                CalculateLevelRecursive(dependentSystem, systemsDict, visited, level + 1);
            }
        }
    }
}