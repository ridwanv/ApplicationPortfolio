// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.OrgSystems.Caching;

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Queries.GetSystemPath;

public class GetSystemPathQuery : ICacheableRequest<Result<List<SystemPathDto>>>
{
    public required int SystemId { get; set; }
    
    public string CacheKey => OrgSystemCacheKey.GetSystemPathCacheKey(SystemId);
    public IEnumerable<string>? Tags => OrgSystemCacheKey.Tags;
}

public class SystemPathDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public bool IsRoot => Level == 0;
    public bool IsLeaf { get; set; }
}

public class GetSystemPathQueryHandler : IRequestHandler<GetSystemPathQuery, Result<List<SystemPathDto>>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public GetSystemPathQueryHandler(IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result<List<SystemPathDto>>> Handle(GetSystemPathQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        var path = new List<SystemPathDto>();
        var currentId = request.SystemId;
        var level = 0;
        
        // Traverse up to root
        while (currentId != 0)
        {
            var system = await db.OrgSystems
                .AsNoTracking()
                .Select(x => new { x.Id, x.Name, x.ParentSystemId })
                .FirstOrDefaultAsync(x => x.Id == currentId, cancellationToken);
                
            if (system == null)
                break;
                
            path.Insert(0, new SystemPathDto 
            { 
                Id = system.Id, 
                Name = system.Name, 
                Level = level,
                IsLeaf = currentId == request.SystemId
            });
            
            currentId = system.ParentSystemId ?? 0;
            level++;
        }
        
        // Adjust levels so root is 0
        if (path.Any())
        {
            var maxLevel = path.Max(p => p.Level);
            foreach (var item in path)
            {
                item.Level = maxLevel - item.Level;
            }
        }
        
        return Result<List<SystemPathDto>>.Success(path);
    }
}