// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Caching;

namespace CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Commands.BulkBackfill;

public class BulkBackfillAttributeCommand : ICacheInvalidatorRequest<Result<BulkBackfillResult>>
{
    public int AttributeDefinitionId { get; set; }
    public string? DefaultValue { get; set; }
    public bool OverwriteExisting { get; set; } = false;
    public List<int>? TargetSystemIds { get; set; } // If null, applies to all systems
    
    public string CacheKey => AttributeDefinitionCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => AttributeDefinitionCacheKey.Tags.Concat(new[] { "orgsystem" });
}

public class BulkBackfillResult
{
    public int SystemsProcessed { get; set; }
    public int SystemsUpdated { get; set; }
    public int SystemsSkipped { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class BulkBackfillAttributeCommandHandler : IRequestHandler<BulkBackfillAttributeCommand, Result<BulkBackfillResult>>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public BulkBackfillAttributeCommandHandler(IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public async Task<Result<BulkBackfillResult>> Handle(BulkBackfillAttributeCommand request, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        
        // Verify attribute definition exists
        var attributeDefinition = await db.AttributeDefinitions
            .FirstOrDefaultAsync(x => x.Id == request.AttributeDefinitionId, cancellationToken);
        
        if (attributeDefinition == null)
            return await Result<BulkBackfillResult>.FailureAsync($"Attribute definition with id: [{request.AttributeDefinitionId}] not found.");

        // Validate default value against data type
        if (!string.IsNullOrWhiteSpace(request.DefaultValue) && 
            !IsValidValueForDataType(request.DefaultValue, attributeDefinition.DataType, attributeDefinition.EnumOptionsCsv))
        {
            return await Result<BulkBackfillResult>.FailureAsync("Default value is not valid for the attribute's data type.");
        }

        var result = new BulkBackfillResult();
        
        // Get target systems
        var systemsQuery = db.OrgSystems.AsQueryable();
        if (request.TargetSystemIds?.Any() == true)
        {
            systemsQuery = systemsQuery.Where(s => request.TargetSystemIds.Contains(s.Id));
        }
        
        var targetSystems = await systemsQuery.ToListAsync(cancellationToken);
        result.SystemsProcessed = targetSystems.Count;

        // Get existing system attributes for this definition
        var existingAttributes = await db.SystemAttributes
            .Where(sa => sa.AttributeDefinitionId == request.AttributeDefinitionId)
            .Where(sa => request.TargetSystemIds == null || request.TargetSystemIds.Contains(sa.OrgSystemId))
            .ToListAsync(cancellationToken);

        var existingAttributesDict = existingAttributes.ToDictionary(sa => sa.OrgSystemId);

        using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var system in targetSystems)
            {
                if (existingAttributesDict.TryGetValue(system.Id, out var existingAttribute))
                {
                    // System already has this attribute
                    if (request.OverwriteExisting || string.IsNullOrEmpty(existingAttribute.Value))
                    {
                        existingAttribute.Value = request.DefaultValue;
                        result.SystemsUpdated++;
                    }
                    else
                    {
                        result.SystemsSkipped++;
                    }
                }
                else
                {
                    // System doesn't have this attribute - create new one
                    var newAttribute = new SystemAttribute
                    {
                        OrgSystemId = system.Id,
                        AttributeDefinitionId = request.AttributeDefinitionId,
                        Value = request.DefaultValue
                    };
                    db.SystemAttributes.Add(newAttribute);
                    result.SystemsUpdated++;
                }
            }

            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return await Result<BulkBackfillResult>.SuccessAsync(result);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            result.Errors.Add($"Transaction failed: {ex.Message}");
            return await Result<BulkBackfillResult>.FailureAsync(result.Errors);
        }
    }

    private bool IsValidValueForDataType(string value, AttributeDataType dataType, string? enumOptionsCsv)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;

        return dataType switch
        {
            AttributeDataType.Text => true,
            AttributeDataType.Number => decimal.TryParse(value, out _),
            AttributeDataType.Boolean => bool.TryParse(value, out _),
            AttributeDataType.Date => DateTime.TryParse(value, out _),
            AttributeDataType.Enum => !string.IsNullOrWhiteSpace(enumOptionsCsv) && 
                                     enumOptionsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                         .Select(x => x.Trim())
                                         .Contains(value, StringComparer.OrdinalIgnoreCase),
            _ => false
        };
    }
}