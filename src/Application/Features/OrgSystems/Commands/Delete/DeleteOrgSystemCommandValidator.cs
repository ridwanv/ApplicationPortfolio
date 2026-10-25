// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Commands.Delete;

public class DeleteOrgSystemCommandValidator : AbstractValidator<DeleteOrgSystemCommand>
{
    public DeleteOrgSystemCommandValidator()
    {
        RuleFor(v => v.Id).NotNull().ForEach(v => v.NotEqual(0));
    }
}


