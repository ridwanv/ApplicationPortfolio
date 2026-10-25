// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class SystemCapability : BaseAuditableEntity
{
    public int SystemId { get; set; }
    public int CapabilityId { get; set; }
    public string? ImplementationLevel { get; set; } // Fully, Partially, Planned
    public string? Notes { get; set; }
    public DateTime? ImplementationDate { get; set; }
    public string? ResponsiblePerson { get; set; }

    // Navigation properties
    public virtual System System { get; set; } = default!;
    public virtual Capability Capability { get; set; } = default!;
}