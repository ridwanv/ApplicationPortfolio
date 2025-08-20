// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Systems.Queries.Analytics;

public class SystemsAnalyticsDto
{
    public int Total { get; set; }
    public Dictionary<string, int> ByLifecycle { get; set; } = new();
    public Dictionary<string, int> ByHosting { get; set; } = new();
    public decimal TotalOpex { get; set; }
    public decimal TotalCapex { get; set; }
}

public class GetSystemsAnalyticsQuery : ICacheableRequest<SystemsAnalyticsDto>
{
    public string CacheKey => "SystemsAnalytics";
    public TimeSpan? Expiration => TimeSpan.FromMinutes(5);
}

public class GetSystemsAnalyticsQueryHandler : IRequestHandler<GetSystemsAnalyticsQuery, SystemsAnalyticsDto>
{
    private readonly IApplicationDbContextFactory _dbFactory;

    public GetSystemsAnalyticsQueryHandler(IApplicationDbContextFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<SystemsAnalyticsDto> Handle(GetSystemsAnalyticsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        var total = await db.ApplicationSystems.CountAsync(cancellationToken);
        var byLifecycle = await db.ApplicationSystems
            .GroupBy(x => x.LifecycleStage ?? "Unknown")
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);
        var byHosting = await db.ApplicationSystems
            .GroupBy(x => x.Hosting ?? "Unknown")
            .Select(g => new { Key = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.Count, cancellationToken);
        var costs = await db.CostEntries
            .GroupBy(x => 1)
            .Select(g => new { Opex = g.Sum(x => x.Opex), Capex = g.Sum(x => x.Capex) })
            .FirstOrDefaultAsync(cancellationToken) ?? new { Opex = 0m, Capex = 0m };

        return new SystemsAnalyticsDto
        {
            Total = total,
            ByLifecycle = byLifecycle,
            ByHosting = byHosting,
            TotalOpex = costs.Opex,
            TotalCapex = costs.Capex
        };
    }
}

