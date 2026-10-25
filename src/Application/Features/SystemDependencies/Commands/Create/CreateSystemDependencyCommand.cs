// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.SystemDependencies.Caching;

namespace CleanArchitecture.Blazor.Application.Features.SystemDependencies.Commands.Create;

public class CreateSystemDependencyCommand : ICacheInvalidatorRequest<Result<int>>
{
    public int SourceSystemId { get; set; }
    public int TargetSystemId { get; set; }
    public string? Notes { get; set; }
    
    public string CacheKey => SystemDependencyCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => SystemDependencyCacheKey.Tags;
}

public class CreateSystemDependencyCommandHandler : IRequestHandler<CreateSystemDependencyCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public CreateSystemDependencyCommandHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(CreateSystemDependencyCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        // Validate systems exist
        var sourceSystem = await db.OrgSystems.FindAsync(request.SourceSystemId, cancellationToken);
        if (sourceSystem == null)
            return await Result<int>.FailureAsync($"Source system with id: [{request.SourceSystemId}] not found.");

        var targetSystem = await db.OrgSystems.FindAsync(request.TargetSystemId, cancellationToken);
        if (targetSystem == null)
            return await Result<int>.FailureAsync($"Target system with id: [{request.TargetSystemId}] not found.");

        // Prevent self-dependency
        if (request.SourceSystemId == request.TargetSystemId)
            return await Result<int>.FailureAsync("A system cannot depend on itself.");

        // Check if dependency already exists
        var existingDependency = await db.DependencyEdges
            .FirstOrDefaultAsync(x => x.SourceSystemId == request.SourceSystemId && x.TargetSystemId == request.TargetSystemId, cancellationToken);
        
        if (existingDependency != null)
            return await Result<int>.FailureAsync($"Dependency from '{sourceSystem.Name}' to '{targetSystem.Name}' already exists.");

        // Check for circular dependencies
        if (await WouldCreateCircularDependency(db, request.SourceSystemId, request.TargetSystemId, cancellationToken))
            return await Result<int>.FailureAsync("Creating this dependency would result in a circular dependency.");

        var entity = _mapper.Map<DependencyEdge>(request);
        db.DependencyEdges.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return await Result<int>.SuccessAsync(entity.Id);
    }

    private async Task<bool> WouldCreateCircularDependency(IApplicationDbContext db, int sourceId, int targetId, CancellationToken cancellationToken)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(sourceId);
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (visited.Contains(current)) continue;
            
            visited.Add(current);
            
            // If we reach the target, there would be a cycle
            if (current == targetId) return true;
            
            // Get all systems that depend on the current system
            var dependents = await db.DependencyEdges
                .Where(d => d.TargetSystemId == current)
                .Select(d => d.SourceSystemId)
                .ToListAsync(cancellationToken);
            
            foreach (var dependent in dependents)
            {
                if (!visited.Contains(dependent))
                    queue.Enqueue(dependent);
            }
        }
        
        return false;
    }
}