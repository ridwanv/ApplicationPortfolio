// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Capability : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Level { get; set; } // Strategic, Tactical, Operational
    public string? Owner { get; set; }
    public string? Status { get; set; } // Active, Deprecated, Planned
    public string? MaturityLevel { get; set; } // Initial, Developing, Defined, Managed, Optimizing
    public int? Priority { get; set; }
    public string? BusinessValue { get; set; }
    public string? Dependencies { get; set; }
    public string? Risks { get; set; }

    // Navigation properties
    public virtual ICollection<SystemCapability> SystemCapabilities { get; set; } = new List<SystemCapability>();
}