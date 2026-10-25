// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Caching;

namespace CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Commands.Delete;

public class DeleteAttributeDefinitionCommand : ICacheInvalidatorRequest<Result>
{
    public DeleteAttributeDefinitionCommand(int[] id)
    {
        Id = id;
    }

    public int[] Id { get; }
    public string CacheKey => AttributeDefinitionCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => AttributeDefinitionCacheKey.Tags;
}

public class DeleteAttributeDefinitionCommandHandler : IRequestHandler<DeleteAttributeDefinitionCommand, Result>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public DeleteAttributeDefinitionCommandHandler(IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result> Handle(DeleteAttributeDefinitionCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        var attributeDefinitions = await db.AttributeDefinitions
            .Where(x => request.Id.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (!attributeDefinitions.Any())
            return await Result.FailureAsync("No attribute definitions found to delete.");

        // Check if any attributes are in use
        foreach (var attrDef in attributeDefinitions)
        {
            var isInUse = await db.SystemAttributes
                .AnyAsync(x => x.AttributeDefinitionId == attrDef.Id, cancellationToken);
            
            if (isInUse)
            {
                return await Result.FailureAsync(
                    $"Cannot delete attribute definition '{attrDef.Name}' because it is being used by one or more systems. " +
                    "Please remove all system attribute values first.");
            }
        }

        // Safe to delete
        foreach (var attrDef in attributeDefinitions)
        {
            attrDef.AddDomainEvent(new DeletedEvent<AttributeDefinition>(attrDef));
            db.AttributeDefinitions.Remove(attrDef);
        }

        await db.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync();
    }
}