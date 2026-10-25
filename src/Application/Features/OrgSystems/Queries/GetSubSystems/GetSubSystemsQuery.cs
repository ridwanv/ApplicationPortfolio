// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.OrgSystems.Caching;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Queries.GetSubSystems;

public class GetSubSystemsQuery : ICacheableRequest<Result<List<OrgSystemDto>>>
{
    public required int ParentSystemId { get; set; }
    public bool IncludeAttributes { get; set; } = false;
    
    public string CacheKey => OrgSystemCacheKey.GetSubSystemsCacheKey(ParentSystemId);
    public IEnumerable<string>? Tags => OrgSystemCacheKey.Tags;
}

public class GetSubSystemsQueryHandler : IRequestHandler<GetSubSystemsQuery, Result<List<OrgSystemDto>>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public GetSubSystemsQueryHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<List<OrgSystemDto>>> Handle(GetSubSystemsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        var query = db.OrgSystems.AsNoTracking()
            .Where(x => x.ParentSystemId == request.ParentSystemId)
            .Include(x => x.ParentSystem)
            .Include(x => x.SubSystems);
            
        if (request.IncludeAttributes)
        {
            query = query.Include(x => x.Attributes)
                         .ThenInclude(attr => attr.AttributeDefinition);
        }
        
        var systems = await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);
        
        var result = _mapper.Map<List<OrgSystemDto>>(systems);
        return Result<List<OrgSystemDto>>.Success(result);
    }
}