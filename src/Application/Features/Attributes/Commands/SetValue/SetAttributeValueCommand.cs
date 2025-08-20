// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Attributes.Commands.SetValue;

public class SetAttributeValueCommand : IRequest<Result<int>>
{
    public int ApplicationSystemId { get; set; }
    public int AttributeDefinitionId { get; set; }
    public string? Text { get; set; }
    public decimal? Number { get; set; }
    public bool? Bool { get; set; }
    public DateTime? Date { get; set; }
}

public class SetAttributeValueCommandHandler : IRequestHandler<SetAttributeValueCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbFactory;

    public SetAttributeValueCommandHandler(IApplicationDbContextFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<Result<int>> Handle(SetAttributeValueCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        var def = await db.AttributeDefinitions.SingleOrDefaultAsync(x => x.Id == request.AttributeDefinitionId, cancellationToken);
        if (def == null) return await Result<int>.FailureAsync("Attribute definition not found.");
        var entity = await db.AttributeValues.SingleOrDefaultAsync(x => x.ApplicationSystemId == request.ApplicationSystemId && x.AttributeDefinitionId == request.AttributeDefinitionId, cancellationToken);
        if (entity == null)
        {
            entity = new AttributeValue
            {
                ApplicationSystemId = request.ApplicationSystemId,
                AttributeDefinitionId = request.AttributeDefinitionId
            };
            db.AttributeValues.Add(entity);
        }

        entity.ValueText = null;
        entity.ValueNumber = null;
        entity.ValueBool = null;
        entity.ValueDate = null;

        switch (def.DataType)
        {
            case AttributeDataType.Text:
            case AttributeDataType.Dropdown:
                entity.ValueText = request.Text;
                break;
            case AttributeDataType.Number:
                entity.ValueNumber = request.Number;
                break;
            case AttributeDataType.Boolean:
                entity.ValueBool = request.Bool;
                break;
            case AttributeDataType.Date:
                entity.ValueDate = request.Date;
                break;
        }

        await db.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(entity.Id);
    }
}

