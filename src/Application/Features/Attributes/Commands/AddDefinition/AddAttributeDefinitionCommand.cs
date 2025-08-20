// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Attributes.Commands.AddDefinition;

public class AddAttributeDefinitionCommand : ICacheInvalidatorRequest<Result<int>>
{
    public string? Name { get; set; }
    public AttributeDataType DataType { get; set; }
    public bool IsRequired { get; set; }
    public List<string>? Options { get; set; }
    public string CacheKey => AttributeCacheKey.GetAllDefinitionsCacheKey;
    public IEnumerable<string>? Tags => AttributeCacheKey.Tags;
}

public class AddAttributeDefinitionCommandHandler : IRequestHandler<AddAttributeDefinitionCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbFactory;

    public AddAttributeDefinitionCommandHandler(IApplicationDbContextFactory dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<Result<int>> Handle(AddAttributeDefinitionCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbFactory.CreateAsync(cancellationToken);
        var exists = await db.AttributeDefinitions.AnyAsync(x => x.Name == request.Name, cancellationToken);
        if (exists) return await Result<int>.FailureAsync($"Attribute '{request.Name}' already exists.");
        var def = new AttributeDefinition
        {
            Name = request.Name,
            DataType = request.DataType,
            IsRequired = request.IsRequired,
            Options = request.Options
        };
        db.AttributeDefinitions.Add(def);
        await db.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(def.Id);
    }
}

