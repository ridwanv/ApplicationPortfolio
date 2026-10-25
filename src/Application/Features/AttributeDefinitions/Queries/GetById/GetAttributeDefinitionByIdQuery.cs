// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Caching;
using CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.DTOs;

namespace CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Queries.GetById;

public class GetAttributeDefinitionByIdQuery : ICacheableRequest<Result<AttributeDefinitionDto?>>
{
    public required int Id { get; set; }
    public bool IncludeUsageCount { get; set; } = true;
    
    public string CacheKey => AttributeDefinitionCacheKey.GetByIdCacheKey(Id);
    public IEnumerable<string>? Tags => AttributeDefinitionCacheKey.Tags;
}

public class GetAttributeDefinitionByIdQueryHandler : IRequestHandler<GetAttributeDefinitionByIdQuery, Result<AttributeDefinitionDto?>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public GetAttributeDefinitionByIdQueryHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<AttributeDefinitionDto?>> Handle(GetAttributeDefinitionByIdQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        var attributeDefinition = await db.AttributeDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        var result = _mapper.Map<AttributeDefinitionDto>(attributeDefinition);

        if (result != null && request.IncludeUsageCount)
        {
            result.SystemsUsingCount = await db.SystemAttributes
                .CountAsync(sa => sa.AttributeDefinitionId == request.Id, cancellationToken);
        }

        return Result<AttributeDefinitionDto?>.Success(result);
    }
}