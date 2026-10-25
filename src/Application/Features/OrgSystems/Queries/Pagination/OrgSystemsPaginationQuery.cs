// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.OrgSystems.Caching;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.DTOs;
using CleanArchitecture.Blazor.Application.Features.OrgSystems.Specifications;

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Queries.Pagination;

public class OrgSystemsWithPaginationQuery : OrgSystemAdvancedFilter, ICacheableRequest<PaginatedData<OrgSystemDto>>
{
    public OrgSystemAdvancedSpecification Specification => new(this);
    public string CacheKey => OrgSystemCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => OrgSystemCacheKey.Tags;

    public override string ToString()
    {
        return $"CurrentUser:{CurrentUser?.UserId},Keyword:{Keyword},Criticality:{Criticality},LifecycleStatus:{LifecycleStatus},OwnerId:{OwnerId},ParentSystemId:{ParentSystemId},HasSubSystems:{HasSubSystems},OnlyRootSystems:{OnlyRootSystems},IncludeSubSystems:{IncludeSubSystems},Tags:{string.Join(",", Tags ?? new List<string>())},SortDirection:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
}

public class OrgSystemsWithPaginationQueryHandler : IRequestHandler<OrgSystemsWithPaginationQuery, PaginatedData<OrgSystemDto>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;
    public OrgSystemsWithPaginationQueryHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<PaginatedData<OrgSystemDto>> Handle(OrgSystemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        // Build query with necessary includes
        var query = db.OrgSystems
            .Include(x => x.ParentSystem)
            .Include(x => x.SubSystems)
            .Include(x => x.Attributes)
                .ThenInclude(attr => attr.AttributeDefinition);
        
        var data = await query.OrderBy($"{request.OrderBy} {request.SortDirection}")
            .ProjectToPaginatedDataAsync<OrgSystem, OrgSystemDto>(request.Specification, request.PageNumber, request.PageSize, _mapper.ConfigurationProvider, cancellationToken);
        return data;
    }
}


