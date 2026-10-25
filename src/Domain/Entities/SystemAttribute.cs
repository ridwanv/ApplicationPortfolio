// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class SystemAttribute : BaseAuditableEntity
{
    public int SystemId { get; set; }
    public int AttributeDefinitionId { get; set; }
    public string? Value { get; set; }

    // Navigation
    public virtual System System { get; set; } = default!;
    public virtual AttributeDefinition AttributeDefinition { get; set; } = default!;
}


