// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class ITSystem : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Owner { get; set; }
    public string? Version { get; set; }
    public string? Status { get; set; }
    public string? Category { get; set; }
    public string? Vendor { get; set; }
    public DateTime? InstallationDate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public string? MaintenanceNotes { get; set; }
    public decimal? AnnualCost { get; set; }
    public string? CriticalityLevel { get; set; }
    public string? ComplianceRequirements { get; set; }
    public int? ParentSystemId { get; set; }

    // Navigation properties
    public virtual ITSystem? ParentSystem { get; set; }
    public virtual ICollection<ITSystem> SubSystems { get; set; } = new List<ITSystem>();
    public virtual ICollection<SystemCapability> SystemCapabilities { get; set; } = new List<SystemCapability>();
}