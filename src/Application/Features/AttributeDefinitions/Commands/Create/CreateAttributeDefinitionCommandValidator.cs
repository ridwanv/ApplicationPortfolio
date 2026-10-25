// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Commands.Create;

public class CreateAttributeDefinitionCommandValidator : AbstractValidator<CreateAttributeDefinitionCommand>
{
    private readonly IApplicationDbContextFactory _dbContextFactory;

    public CreateAttributeDefinitionCommandValidator(IApplicationDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(256).WithMessage("Name must not exceed 256 characters.")
            .MustAsync(BeUniqueName).WithMessage("The specified name already exists.");

        RuleFor(v => v.Description)
            .MaximumLength(2048).WithMessage("Description must not exceed 2048 characters.");

        RuleFor(v => v.DataType)
            .IsInEnum().WithMessage("Invalid data type.");

        RuleFor(v => v.EnumOptionsCsv)
            .NotEmpty().When(v => v.DataType == AttributeDataType.Enum)
            .WithMessage("Enum options are required when data type is Enum.")
            .Must(BeValidEnumOptions).When(v => !string.IsNullOrWhiteSpace(v.EnumOptionsCsv))
            .WithMessage("Enum options must be comma-separated values without duplicates.");

        RuleFor(v => v.DefaultValue)
            .Must((cmd, defaultValue) => BeValidValueForDataType(defaultValue, cmd.DataType, cmd.EnumOptionsCsv))
            .When(v => !string.IsNullOrWhiteSpace(v.DefaultValue))
            .WithMessage("Default value is not valid for the specified data type.");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        await using var db = await _dbContextFactory.CreateAsync(cancellationToken);
        return !await db.AttributeDefinitions.AnyAsync(x => x.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    private bool BeValidEnumOptions(string? enumOptionsCsv)
    {
        if (string.IsNullOrWhiteSpace(enumOptionsCsv)) return false;
        
        var options = enumOptionsCsv.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();

        // Check for duplicates and empty values
        return options.Count > 0 && options.Count == options.Distinct().Count();
    }

    private bool BeValidValueForDataType(string? value, AttributeDataType dataType, string? enumOptionsCsv)
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