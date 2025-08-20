// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Systems.Queries.Hierarchy;

public class SystemNodeDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<SystemNodeDto> Children { get; set; } = new();
    public decimal TotalOpex { get; set; }
    public decimal TotalCapex { get; set; }
    public int? ValueScore { get; set; }
}

public class GetSystemHierarchyQuery : ICacheableRequest<SystemNodeDto?>
{
    public required int SystemId { get; set; }
    public string CacheKey => $"SystemHierarchy,{SystemId}";
    public IEnumerable<string>? Tags => SystemCacheKey.Tags;
}

public class GetSystemHierarchyQueryHandler : IRequestHandler<GetSystemHierarchyQuery, SystemNodeDto?>
{
    private readonly IApplicationDbContextFactory _dbFactory;

    public GetSystemHierarchyQueryHandler(IApplicationDbContextFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<SystemNodeDto?> Handle(GetSystemHierarchyQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        var root = await db.ApplicationSystems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.SystemId, cancellationToken);
        if (root == null) return null;

        async Task<SystemNodeDto> BuildAsync(ApplicationSystem node, CancellationToken ct)
        {
            var childEntities = await db.ApplicationSystems.AsNoTracking().Where(x => x.ParentId == node.Id).ToListAsync(ct);
            var cost = await db.CostEntries.Where(c => c.ApplicationSystemId == node.Id)
                .GroupBy(c => c.ApplicationSystemId)
                .Select(g => new { Opex = g.Sum(x => x.Opex), Capex = g.Sum(x => x.Capex) })
                .FirstOrDefaultAsync(ct) ?? new { Opex = 0m, Capex = 0m };
            var value = await db.ValueRatings.Where(v => v.ApplicationSystemId == node.Id).Select(v => v.Score).FirstOrDefaultAsync(ct);

            var dto = new SystemNodeDto
            {
                Id = node.Id,
                Name = node.Name,
                TotalOpex = cost.Opex,
                TotalCapex = cost.Capex,
                ValueScore = value == 0 ? null : value
            };
            foreach (var child in childEntities)
            {
                dto.Children.Add(await BuildAsync(child, ct));
            }
            // roll-up
            dto.TotalOpex += dto.Children.Sum(c => c.TotalOpex);
            dto.TotalCapex += dto.Children.Sum(c => c.TotalCapex);
            return dto;
        }

        return await BuildAsync(root, cancellationToken);
    }
}

