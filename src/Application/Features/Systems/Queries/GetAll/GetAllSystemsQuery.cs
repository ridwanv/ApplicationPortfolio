// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Systems.Caching;
using CleanArchitecture.Blazor.Application.Features.Systems.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.Systems.Queries.GetAll;

public class GetAllSystemsQuery : ICacheableRequest<IEnumerable<SystemDto>>
{
    public string CacheKey => SystemCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => SystemCacheKey.Tags;
}

public class GetSystemByIdQuery : ICacheableRequest<SystemDto?>
{
    public required int Id { get; set; }
    public string CacheKey => SystemCacheKey.GetByIdCacheKey(Id);
    public IEnumerable<string>? Tags => SystemCacheKey.Tags;
}

public class GetAllSystemsQueryHandler :
    IRequestHandler<GetAllSystemsQuery, IEnumerable<SystemDto>>,
    IRequestHandler<GetSystemByIdQuery, SystemDto?>
{
    private readonly IMapper _mapper;
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public GetAllSystemsQueryHandler(IMapper mapper, IApplicationDbContextFactory dbContextFactory)
    {
        _mapper = mapper;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<IEnumerable<SystemDto>> Handle(GetAllSystemsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return await db.ApplicationSystems
            .ProjectTo<SystemDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
    }

    public async Task<SystemDto?> Handle(GetSystemByIdQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return await db.ApplicationSystems
            .Where(x => x.Id == request.Id)
            .ProjectTo<SystemDto>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }
}