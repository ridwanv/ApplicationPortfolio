// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Systems.Commands.Workflow;

public class ReviewLifecycleChangeRequestCommand : IRequest<Result<int>>
{
    public required int RequestId { get; set; }
    public required bool Approve { get; set; }
    public string? Comments { get; set; }
}

public class ReviewLifecycleChangeRequestCommandHandler : IRequestHandler<ReviewLifecycleChangeRequestCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbFactory;
    private readonly IMailService _mailService;

    public ReviewLifecycleChangeRequestCommandHandler(IApplicationDbContextFactory dbFactory, IMailService mailService)
    {
        _dbFactory = dbFactory;
        _mailService = mailService;
    }

    public async Task<Result<int>> Handle(ReviewLifecycleChangeRequestCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        var entity = await db.LifecycleChangeRequests.FirstOrDefaultAsync(x => x.Id == request.RequestId, cancellationToken);
        if (entity == null) return await Result<int>.FailureAsync("Request not found");
        entity.Status = request.Approve ? LifecycleRequestStatus.Approved : LifecycleRequestStatus.Rejected;
        entity.Comments = request.Comments;
        if (request.Approve)
        {
            var system = await db.ApplicationSystems.FirstOrDefaultAsync(x => x.Id == entity.ApplicationSystemId, cancellationToken);
            if (system != null) system.LifecycleStage = entity.ToStage;
        }
        await db.SaveChangesAsync(cancellationToken);
        // Notify via email - placeholder recipient, integrate owner email when available
        var subject = request.Approve ? "Lifecycle request approved" : "Lifecycle request rejected";
        var body = $"Request #{entity.Id} {(request.Approve ? "approved" : "rejected")} with comments: {request.Comments}";
        try { await _mailService.SendAsync("owner@example.com", subject, body); } catch { /* ignore send failures */ }
        return await Result<int>.SuccessAsync(entity.Id);
    }
}

