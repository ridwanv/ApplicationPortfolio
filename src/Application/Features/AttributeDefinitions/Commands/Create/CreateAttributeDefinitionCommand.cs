// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Caching;

namespace CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Commands.Create;

public class CreateAttributeDefinitionCommand : ICacheInvalidatorRequest<Result<int>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public AttributeDataType DataType { get; set; }
    public bool IsRequired { get; set; }
    public string? EnumOptionsCsv { get; set; }
    public bool BackfillExistingSystems { get; set; } = false;
    public string? DefaultValue { get; set; }
    
    public string CacheKey => AttributeDefinitionCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => AttributeDefinitionCacheKey.Tags;
}

public class CreateAttributeDefinitionCommandHandler : IRequestHandler<CreateAttributeDefinitionCommand, Result<int>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;
    private readonly IMapper _mapper;

    public CreateAttributeDefinitionCommandHandler(IApplicationDbContextFactory dbContextFactory, IMapper mapper)
    {
        _dbContextFactory = dbContextFactory;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(CreateAttributeDefinitionCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        // Check if attribute name already exists
        var existingAttribute = await db.AttributeDefinitions
            .FirstOrDefaultAsync(x => x.Name.ToLower() == request.Name.ToLower(), cancellationToken);
        
        if (existingAttribute != null)
            return await Result<int>.FailureAsync($"Attribute definition with name '{request.Name}' already exists.");

        using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var entity = _mapper.Map<AttributeDefinition>(request);
            db.AttributeDefinitions.Add(entity);
            await db.SaveChangesAsync(cancellationToken);

            // Backfill existing systems if requested
            if (request.BackfillExistingSystems)
            {
                var allSystems = await db.OrgSystems.ToListAsync(cancellationToken);
                var systemAttributes = allSystems.Select(system => new SystemAttribute
                {
                    OrgSystemId = system.Id,
                    AttributeDefinitionId = entity.Id,
                    Value = request.DefaultValue
                }).ToList();

                db.SystemAttributes.AddRange(systemAttributes);
                await db.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
            return await Result<int>.SuccessAsync(entity.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}