// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class OrgSystem : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? BusinessOwnerId { get; set; }
    public int? TechnicalOwnerId { get; set; }
    public SystemCriticality Criticality { get; set; } = SystemCriticality.Medium;
    public SystemLifecycleStatus LifecycleStatus { get; set; } = SystemLifecycleStatus.Active;
    public List<string>? Tags { get; set; }
    public string? Dependencies { get; set; }
    
    // Hierarchy support
    public int? ParentSystemId { get; set; }

    // Navigation properties
    public virtual OrgSystem? ParentSystem { get; set; }
    public virtual ICollection<OrgSystem> SubSystems { get; set; } = new List<OrgSystem>();
    public virtual ICollection<SystemAttribute> Attributes { get; set; } = new List<SystemAttribute>();
    public virtual ICollection<DependencyEdge> OutgoingDependencies { get; set; } = new List<DependencyEdge>();
    public virtual ICollection<DependencyEdge> IncomingDependencies { get; set; } = new List<DependencyEdge>();
}


