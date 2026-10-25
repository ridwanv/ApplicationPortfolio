// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.OrgSystems.Caching;

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Commands.Delete;

public class DeleteOrgSystemCommand : ICacheInvalidatorRequest<Result>
{
    public DeleteOrgSystemCommand(int[] id)
    {
        Id = id;
    }

    public int[] Id { get; }
    public string CacheKey => OrgSystemCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => OrgSystemCacheKey.Tags;
}

public class DeleteOrgSystemCommandHandler : IRequestHandler<DeleteOrgSystemCommand, Result>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public DeleteOrgSystemCommandHandler(IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result> Handle(DeleteOrgSystemCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        var items = await db.OrgSystems.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        
        // Check for sub-systems that would be orphaned
        foreach (var item in items)
        {
            var hasSubSystems = await db.OrgSystems.AnyAsync(x => x.ParentSystemId == item.Id, cancellationToken);
            if (hasSubSystems)
            {
                return await Result.FailureAsync($"Cannot delete system '{item.Name}' because it has sub-systems. Please move or delete sub-systems first.");
            }
            
            // Check for dependencies that would be broken
            var hasDependencies = await db.DependencyEdges.AnyAsync(x => x.SourceSystemId == item.Id || x.TargetSystemId == item.Id, cancellationToken);
            if (hasDependencies)
            {
                return await Result.FailureAsync($"Cannot delete system '{item.Name}' because it has active dependencies. Please remove dependencies first.");
            }
        }
        
        foreach (var item in items)
        {
            item.AddDomainEvent(new DeletedEvent<OrgSystem>(item));
            db.OrgSystems.Remove(item);
        }
        await db.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}


