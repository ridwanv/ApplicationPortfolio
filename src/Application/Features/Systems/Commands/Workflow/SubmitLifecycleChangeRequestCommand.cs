// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Systems.Commands.Workflow;

public class SubmitLifecycleChangeRequestCommand : IRequest<Result<int>>
{
    public required int SystemId { get; set; }
    public required string FromStage { get; set; }
    public required string ToStage { get; set; }
    public string? Comments { get; set; }
}

public class LifecycleChangeRequestCreatedEvent : DomainEvent
{
    public required int RequestId { get; init; }
    public required int SystemId { get; init; }
    public required string ToStage { get; init; }
}

public class SubmitLifecycleChangeRequestCommandHandler : IRequestHandler<SubmitLifecycleChangeRequestCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbFactory;

    public SubmitLifecycleChangeRequestCommandHandler(IApplicationDbContextFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<Result<int>> Handle(SubmitLifecycleChangeRequestCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        var entity = new LifecycleChangeRequest
        {
            ApplicationSystemId = request.SystemId,
            FromStage = request.FromStage,
            ToStage = request.ToStage,
            Comments = request.Comments,
            Status = LifecycleRequestStatus.Submitted
        };
        db.LifecycleChangeRequests.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        entity.AddDomainEvent(new LifecycleChangeRequestCreatedEvent { RequestId = entity.Id, SystemId = entity.ApplicationSystemId, ToStage = entity.ToStage ?? string.Empty });
        return await Result<int>.SuccessAsync(entity.Id);
    }
}

