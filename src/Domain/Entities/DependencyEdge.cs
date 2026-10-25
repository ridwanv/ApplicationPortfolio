// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class DependencyEdge : BaseAuditableEntity
{
    public int SourceSystemId { get; set; }
    public int TargetSystemId { get; set; }
    public string? Notes { get; set; }

    public virtual OrgSystem SourceSystem { get; set; } = default!;
    public virtual OrgSystem TargetSystem { get; set; } = default!;
}


