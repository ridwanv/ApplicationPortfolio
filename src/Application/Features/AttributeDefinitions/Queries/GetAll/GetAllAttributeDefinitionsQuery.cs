// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Caching;
using CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Queries.GetAll;

public class GetAllAttributeDefinitionsQuery : ICacheableRequest<Result<List<AttributeDefinitionDto>>>
{
    public bool IncludeUsageCount { get; set; } = true;
    
    public string CacheKey => AttributeDefinitionCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => AttributeDefinitionCacheKey.Tags;
}

public class GetAllAttributeDefinitionsQueryHandler : IRequestHandler<GetAllAttributeDefinitionsQuery, Result<List<AttributeDefinitionDto>>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public GetAllAttributeDefinitionsQueryHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<List<AttributeDefinitionDto>>> Handle(GetAllAttributeDefinitionsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        var attributeDefinitions = await db.AttributeDefinitions
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<AttributeDefinitionDto>>(attributeDefinitions);

        if (request.IncludeUsageCount)
        {
            // Get usage counts
            var usageCounts = await db.SystemAttributes
                .GroupBy(sa => sa.AttributeDefinitionId)
                .Select(g => new { AttributeDefinitionId = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var usageDict = usageCounts.ToDictionary(x => x.AttributeDefinitionId, x => x.Count);

            foreach (var dto in result)
            {
                dto.SystemsUsingCount = usageDict.GetValueOrDefault(dto.Id, 0);
            }
        }

        return Result<List<AttributeDefinitionDto>>.Success(result);
    }
}