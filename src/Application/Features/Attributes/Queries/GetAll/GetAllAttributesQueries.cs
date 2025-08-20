// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.Attributes.Caching;

namespace CleanArchitecture.Blazor.Application.Features.Attributes.Queries.GetAll;

public class AttributeDefinitionDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public AttributeDataType DataType { get; set; }
    public bool IsRequired { get; set; }
    public List<string>? Options { get; set; }
}

public class AttributeValueDto
{
    public int Id { get; set; }
    public int AttributeDefinitionId { get; set; }
    public string? Text { get; set; }
    public decimal? Number { get; set; }
    public bool? Bool { get; set; }
    public DateTime? Date { get; set; }
}

public class GetAllAttributeDefinitionsQuery : ICacheableRequest<IEnumerable<AttributeDefinitionDto>>
{
    public string CacheKey => AttributeCacheKey.GetAllDefinitionsCacheKey;
    public IEnumerable<string>? Tags => AttributeCacheKey.Tags;
}

public class GetAttributeDefinitionByIdQuery : ICacheableRequest<AttributeDefinitionDto?>
{
    public required int Id { get; set; }
    public string CacheKey => AttributeCacheKey.GetDefinitionByIdCacheKey(Id);
    public IEnumerable<string>? Tags => AttributeCacheKey.Tags;
}

public class GetAttributeValuesBySystemQuery : ICacheableRequest<IEnumerable<AttributeValueDto>>
{
    public required int SystemId { get; set; }
    public string CacheKey => AttributeCacheKey.GetValuesBySystemCacheKey(SystemId);
    public IEnumerable<string>? Tags => AttributeCacheKey.Tags;
}

public class GetAllAttributesQueryHandler :
    IRequestHandler<GetAllAttributeDefinitionsQuery, IEnumerable<AttributeDefinitionDto>>,
    IRequestHandler<GetAttributeDefinitionByIdQuery, AttributeDefinitionDto?>,
    IRequestHandler<GetAttributeValuesBySystemQuery, IEnumerable<AttributeValueDto>>
{
    private readonly IApplicationDbContextFactory _dbFactory;

    public GetAllAttributesQueryHandler(IApplicationDbContextFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<IEnumerable<AttributeDefinitionDto>> Handle(GetAllAttributeDefinitionsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        return await db.AttributeDefinitions
            .Select(x => new AttributeDefinitionDto
            {
                Id = x.Id,
                Name = x.Name,
                DataType = x.DataType,
                IsRequired = x.IsRequired,
                Options = x.Options
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<AttributeDefinitionDto?> Handle(GetAttributeDefinitionByIdQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        return await db.AttributeDefinitions
            .Where(x => x.Id == request.Id)
            .Select(x => new AttributeDefinitionDto
            {
                Id = x.Id,
                Name = x.Name,
                DataType = x.DataType,
                IsRequired = x.IsRequired,
                Options = x.Options
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<AttributeValueDto>> Handle(GetAttributeValuesBySystemQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        return await db.AttributeValues
            .Where(x => x.ApplicationSystemId == request.SystemId)
            .Select(x => new AttributeValueDto
            {
                Id = x.Id,
                AttributeDefinitionId = x.AttributeDefinitionId,
                Text = x.ValueText,
                Number = x.ValueNumber,
                Bool = x.ValueBool,
                Date = x.ValueDate
            })
            .ToListAsync(cancellationToken);
    }
}