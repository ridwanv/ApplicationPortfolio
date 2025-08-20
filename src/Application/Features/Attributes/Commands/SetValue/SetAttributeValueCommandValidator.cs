// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Attributes.Commands.SetValue;

public class SetAttributeValueCommandValidator : AbstractValidator<SetAttributeValueCommand>
{
    public SetAttributeValueCommandValidator()
    {
        RuleFor(v => v.ApplicationSystemId).GreaterThan(0);
        RuleFor(v => v.AttributeDefinitionId).GreaterThan(0);
    }
}

