// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Caching;

namespace CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Commands.Update;

public class UpdateAttributeDefinitionCommand : ICacheInvalidatorRequest<Result<int>>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AttributeDataType DataType { get; set; }
    public bool IsRequired { get; set; }
    public string? EnumOptionsCsv { get; set; }
    
    public string CacheKey => AttributeDefinitionCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => AttributeDefinitionCacheKey.Tags;
}

public class UpdateAttributeDefinitionCommandHandler : IRequestHandler<UpdateAttributeDefinitionCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public UpdateAttributeDefinitionCommandHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(UpdateAttributeDefinitionCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        var entity = await db.AttributeDefinitions.FindAsync(request.Id, cancellationToken);
        if (entity == null)
            return await Result<int>.FailureAsync($"Attribute definition with id: [{request.Id}] not found.");

        // Check if new name conflicts with existing (excluding current entity)
        var nameConflict = await db.AttributeDefinitions
            .AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == request.Name.ToLower(), cancellationToken);
        
        if (nameConflict)
            return await Result<int>.FailureAsync($"Attribute definition with name '{request.Name}' already exists.");

        // Check if data type change would affect existing system attributes
        if (entity.DataType != request.DataType)
        {
            var hasExistingValues = await db.SystemAttributes
                .AnyAsync(x => x.AttributeDefinitionId == request.Id && !string.IsNullOrEmpty(x.Value), cancellationToken);
            
            if (hasExistingValues)
            {
                return await Result<int>.FailureAsync(
                    "Cannot change data type because this attribute has existing values in systems. " +
                    "Please clear all existing values first or create a new attribute definition.");
            }
        }

        _mapper.Map(request, entity);
        await db.SaveChangesAsync(cancellationToken);
        
        return await Result<int>.SuccessAsync(entity.Id);
    }
}