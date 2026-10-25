// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.OrgSystems.Caching;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Queries.GetByName;

public class GetSystemByNameQuery : ICacheableRequest<Result<OrgSystemDto?>>
{
    public required string Name { get; set; }
    public bool IncludeAttributes { get; set; } = true;
    
    public string CacheKey => OrgSystemCacheKey.GetByNameCacheKey(Name);
    public IEnumerable<string>? Tags => OrgSystemCacheKey.Tags;
}

public class GetSystemByNameQueryHandler : IRequestHandler<GetSystemByNameQuery, Result<OrgSystemDto?>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public GetSystemByNameQueryHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<OrgSystemDto?>> Handle(GetSystemByNameQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        var query = db.OrgSystems.AsNoTracking()
            .Include(x => x.ParentSystem)
            .Include(x => x.SubSystems);
            
        if (request.IncludeAttributes)
        {
            query = query.Include(x => x.Attributes)
                         .ThenInclude(attr => attr.AttributeDefinition);
        }
        
        var system = await query.FirstOrDefaultAsync(x => x.Name == request.Name, cancellationToken);
        
        var result = _mapper.Map<OrgSystemDto>(system);
        return Result<OrgSystemDto?>.Success(result);
    }
}