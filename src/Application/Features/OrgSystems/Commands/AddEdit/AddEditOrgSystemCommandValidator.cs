// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Commands.AddEdit;

public class AddEditOrgSystemCommandValidator : AbstractValidator<AddEditOrgSystemCommand>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public AddEditOrgSystemCommandValidator(IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("System name is required.")
            .MaximumLength(256).WithMessage("System name must not exceed 256 characters.")
            .MustAsync(BeUniqueName).WithMessage("The specified system name already exists.");

        RuleFor(v => v.Description)
            .MaximumLength(2048).WithMessage("Description must not exceed 2048 characters.");

        RuleFor(v => v.Dependencies)
            .MaximumLength(2048).WithMessage("Dependencies field must not exceed 2048 characters.");

        RuleFor(v => v.BusinessOwnerId)
            .GreaterThan(0).When(v => v.BusinessOwnerId.HasValue)
            .WithMessage("Business Owner ID must be greater than 0.");

        RuleFor(v => v.TechnicalOwnerId)
            .GreaterThan(0).When(v => v.TechnicalOwnerId.HasValue)
            .WithMessage("Technical Owner ID must be greater than 0.");

        RuleFor(v => v.ParentSystemId)
            .GreaterThan(0).When(v => v.ParentSystemId.HasValue)
            .WithMessage("Parent System ID must be greater than 0.")
            .MustAsync(ParentSystemExists).When(v => v.ParentSystemId.HasValue)
            .WithMessage("The specified parent system does not exist.")
            .MustAsync(NotCreateCircularReference).When(v => v.ParentSystemId.HasValue && v.Id > 0)
            .WithMessage("Setting this parent would create a circular reference in the system hierarchy.");

        RuleFor(v => v.Criticality)
            .IsInEnum().WithMessage("Invalid criticality level.");

        RuleFor(v => v.LifecycleStatus)
            .IsInEnum().WithMessage("Invalid lifecycle status.");

        RuleFor(v => v.Tags)
            .Must(HaveValidTags).When(v => v.Tags != null && v.Tags.Any())
            .WithMessage("Tags must not be empty and each tag must not exceed 50 characters.");
    }

    private async Task<bool> BeUniqueName(AddEditOrgSystemCommand command, string name, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return !await db.OrgSystems.AnyAsync(x => x.Id != command.Id && x.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    private async Task<bool> ParentSystemExists(AddEditOrgSystemCommand command, int? parentSystemId, CancellationToken cancellationToken)
    {
        if (!parentSystemId.HasValue) return true;
        
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return await db.OrgSystems.AnyAsync(x => x.Id == parentSystemId.Value, cancellationToken);
    }

    private async Task<bool> NotCreateCircularReference(AddEditOrgSystemCommand command, int? parentSystemId, CancellationToken cancellationToken)
    {
        if (!parentSystemId.HasValue || command.Id <= 0) return true;
        
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return !await WouldCreateCircularReference(db, command.Id, parentSystemId.Value, cancellationToken);
    }

    private async Task<bool> WouldCreateCircularReference(IApplicationDbContext db, int systemId, int parentId, CancellationToken cancellationToken)
    {
        var visited = new HashSet<int>();
        var current = parentId;
        
        while (current != 0)
        {
            if (current == systemId) return true;
            if (visited.Contains(current)) return true;
            
            visited.Add(current);
            var parent = await db.OrgSystems
                .Where(x => x.Id == current)
                .Select(x => x.ParentSystemId)
                .FirstOrDefaultAsync(cancellationToken);
            
            current = parent ?? 0;
        }
        
        return false;
    }

    private bool HaveValidTags(List<string>? tags)
    {
        if (tags == null) return true;
        
        return tags.All(tag => !string.IsNullOrWhiteSpace(tag) && tag.Length <= 50);
    }
}


