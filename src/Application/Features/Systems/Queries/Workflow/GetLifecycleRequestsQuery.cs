// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Systems.Queries.Workflow;

public class LifecycleRequestDto
{
    public int Id { get; set; }
    public int ApplicationSystemId { get; set; }
    public string? FromStage { get; set; }
    public string? ToStage { get; set; }
    public string? RequestedBy { get; set; }
    public string? Comments { get; set; }
    public LifecycleRequestStatus Status { get; set; }
}

public class GetLifecycleRequestsQuery : IRequest<IEnumerable<LifecycleRequestDto>>
{
    public LifecycleRequestStatus? Status { get; set; }
}

public class GetLifecycleRequestsQueryHandler : IRequestHandler<GetLifecycleRequestsQuery, IEnumerable<LifecycleRequestDto>>
{
    private readonly IApplicationDbContextFactory _dbFactory;

    public GetLifecycleRequestsQueryHandler(IApplicationDbContextFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<IEnumerable<LifecycleRequestDto>> Handle(GetLifecycleRequestsQuery request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        var query = db.LifecycleChangeRequests.AsQueryable();
        if (request.Status.HasValue) query = query.Where(x => x.Status == request.Status.Value);
        return await query.AsNoTracking().OrderByDescending(x => x.Created)
            .Select(x => new LifecycleRequestDto
            {
                Id = x.Id,
                ApplicationSystemId = x.ApplicationSystemId,
                FromStage = x.FromStage,
                ToStage = x.ToStage,
                RequestedBy = x.RequestedBy,
                Comments = x.Comments,
                Status = x.Status
            }).ToListAsync(cancellationToken);
    }
}

