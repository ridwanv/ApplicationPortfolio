// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Systems.Commands.AddEdit;

public class AddEditSystemCommandValidator : AbstractValidator<AddEditSystemCommand>
{
    public AddEditSystemCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(180)
            .NotEmpty();
        RuleFor(v => v.Category)
            .MaximumLength(80);
        RuleFor(v => v.Hosting)
            .MaximumLength(80);
        RuleFor(v => v.LifecycleStage)
            .MaximumLength(80);
        RuleFor(v => v.Description)
            .MaximumLength(1024);
    }
}

