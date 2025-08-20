// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Attributes.Queries.Completion;

public class AttributeCompletionResult
{
    public int AttributeDefinitionId { get; set; }
    public string? AttributeName { get; set; }
    public int TotalSystems { get; set; }
    public int Completed { get; set; }
    public int Missing => Math.Max(0, TotalSystems - Completed);
    public decimal CompletionPercent => TotalSystems == 0 ? 0 : Math.Round((decimal)Completed * 100 / TotalSystems, 2);
    public List<SystemDto> MissingSystems { get; set; } = new();
}

public class GetAttributeCompletionQuery : ICacheableRequest<AttributeCompletionResult>
{
    public required int AttributeDefinitionId { get; set; }
    public string CacheKey => $"AttributeCompletion,{AttributeDefinitionId}";
    public IEnumerable<string>? Tags => AttributeCacheKey.Tags;
}

public class GetAttributeCompletionQueryHandler : IRequestHandler<GetAttributeCompletionQuery, AttributeCompletionResult>
{
    private readonly IApplicationDbContextFactory _dbFactory;
    private readonly IMapper _mapper;

    public GetAttributeCompletionQueryHandler(IApplicationDbContextFactory dbFactory, IMapper mapper)
    {
        _dbFactory = dbFactory;
        _mapper = mapper;
    }

    public async Task<AttributeCompletionResult> Handle(GetAttributeCompletionQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        var total = await db.ApplicationSystems.CountAsync(cancellationToken);
        var completedSystemIds = await db.AttributeValues
            .Where(v => v.AttributeDefinitionId == request.AttributeDefinitionId)
            .Select(v => v.ApplicationSystemId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var completed = completedSystemIds.Count;
        var missing = await db.ApplicationSystems
            .Where(s => !completedSystemIds.Contains(s.Id))
            .ProjectTo<SystemDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var attr = await db.AttributeDefinitions.Where(x => x.Id == request.AttributeDefinitionId).FirstOrDefaultAsync(cancellationToken);
        return new AttributeCompletionResult
        {
            AttributeDefinitionId = request.AttributeDefinitionId,
            AttributeName = attr?.Name,
            TotalSystems = total,
            Completed = completed,
            MissingSystems = missing
        };
    }
}

