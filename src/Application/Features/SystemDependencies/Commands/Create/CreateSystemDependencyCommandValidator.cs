// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.SystemDependencies.Commands.Create;

public class CreateSystemDependencyCommandValidator : AbstractValidator<CreateSystemDependencyCommand>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public CreateSystemDependencyCommandValidator(IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;

        RuleFor(v => v.SourceSystemId)
            .GreaterThan(0).WithMessage("Source system ID must be greater than 0.")
            .MustAsync(SourceSystemExists).WithMessage("The specified source system does not exist.");

        RuleFor(v => v.TargetSystemId)
            .GreaterThan(0).WithMessage("Target system ID must be greater than 0.")
            .MustAsync(TargetSystemExists).WithMessage("The specified target system does not exist.");

        RuleFor(v => v)
            .Must(NotBeSelfDependency).WithMessage("A system cannot depend on itself.")
            .MustAsync(NotAlreadyExist).WithMessage("This dependency already exists.")
            .MustAsync(NotCreateCircularDependency).WithMessage("Creating this dependency would result in a circular dependency.");

        RuleFor(v => v.Notes)
            .MaximumLength(2048).WithMessage("Notes must not exceed 2048 characters.");
    }

    private async Task<bool> SourceSystemExists(int sourceSystemId, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return await db.OrgSystems.AnyAsync(x => x.Id == sourceSystemId, cancellationToken);
    }

    private async Task<bool> TargetSystemExists(int targetSystemId, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return await db.OrgSystems.AnyAsync(x => x.Id == targetSystemId, cancellationToken);
    }

    private bool NotBeSelfDependency(CreateSystemDependencyCommand command)
    {
        return command.SourceSystemId != command.TargetSystemId;
    }

    private async Task<bool> NotAlreadyExist(CreateSystemDependencyCommand command, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return !await db.DependencyEdges.AnyAsync(x => 
            x.SourceSystemId == command.SourceSystemId && 
            x.TargetSystemId == command.TargetSystemId, cancellationToken);
    }

    private async Task<bool> NotCreateCircularDependency(CreateSystemDependencyCommand command, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return !await WouldCreateCircularDependency(db, command.SourceSystemId, command.TargetSystemId, cancellationToken);
    }

    private async Task<bool> WouldCreateCircularDependency(IApplicationDbContext db, int sourceId, int targetId, CancellationToken cancellationToken)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(sourceId);
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (visited.Contains(current)) continue;
            
            visited.Add(current);
            
            // If we reach the target, there would be a cycle
            if (current == targetId) return true;
            
            // Get all systems that depend on the current system
            var dependents = await db.DependencyEdges
                .Where(d => d.TargetSystemId == current)
                .Select(d => d.SourceSystemId)
                .ToListAsync(cancellationToken);
            
            foreach (var dependent in dependents)
            {
                if (!visited.Contains(dependent))
                    queue.Enqueue(dependent);
            }
        }
        
        return false;
    }
}