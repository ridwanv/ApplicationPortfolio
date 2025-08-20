// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class ApplicationSystem : BaseAuditableSoftDeleteEntity
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Hosting { get; set; }
    public string? LifecycleStage { get; set; }

    public int? ParentId { get; set; }
    public ApplicationSystem? Parent { get; set; }
    public ICollection<ApplicationSystem>? Children { get; set; }
}

