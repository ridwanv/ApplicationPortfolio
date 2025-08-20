// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Systems.DTOs;
using CleanArchitecture.Blazor.Application.Features.Systems.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Systems.Queries.GetPaged;

public class GetPagedSystemsQuery : IRequest<PaginatedData<SystemDto>>, ICacheableRequest<PaginatedData<SystemDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Search { get; init; }
    public string CacheKey => SystemCacheKey.GetPaginationCacheKey($"page={PageNumber},size={PageSize},q={Search}");
    public TimeSpan? Expiration => TimeSpan.FromMinutes(2);
}

public class GetPagedSystemsQueryHandler : IRequestHandler<GetPagedSystemsQuery, PaginatedData<SystemDto>>
{
    private readonly IApplicationDbContextFactory _dbFactory;
    private readonly IMapper _mapper;

    public GetPagedSystemsQueryHandler(IApplicationDbContextFactory dbFactory, IMapper mapper)
    {
        _dbFactory = dbFactory;
        _mapper = mapper;
    }

    public async Task<PaginatedData<SystemDto>> Handle(GetPagedSystemsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        var query = db.ApplicationSystems.AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.ToLower();
            query = query.Where(x => (x.Name ?? "").ToLower().Contains(term) || (x.Category ?? "").ToLower().Contains(term));
        }
        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(x => x.Name)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<SystemDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        return new PaginatedData<SystemDto>(items, total, request.PageNumber, request.PageSize);
    }
}

