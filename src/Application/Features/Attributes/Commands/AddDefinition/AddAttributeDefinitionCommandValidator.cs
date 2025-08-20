// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Attributes.Commands.AddDefinition;

public class AddAttributeDefinitionCommandValidator : AbstractValidator<AddAttributeDefinitionCommand>
{
    public AddAttributeDefinitionCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(120)
            .NotEmpty();
        RuleFor(v => v.Options)
            .NotEmpty()
            .When(v => v.DataType == AttributeDataType.Dropdown)
            .WithMessage("Options are required for Dropdown attributes");
    }
}

