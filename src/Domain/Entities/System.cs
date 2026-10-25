// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Domain.Entities;

/// <summary>
/// Core System entity for Application Portfolio Management
/// Represents any system, application, or subsystem in the enterprise catalog
/// </summary>
public class System : BaseAuditableEntity
{
    /// <summary>
    /// System name - required unique identifier
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Detailed description of the system
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// System type classification
    /// </summary>
    public SystemType SystemType { get; set; } = SystemType.Application;
    
    /// <summary>
    /// Current lifecycle status
    /// </summary>
    public SystemLifecycleStatus LifecycleStatus { get; set; } = SystemLifecycleStatus.Active;
    
    /// <summary>
    /// Business criticality level
    /// </summary>
    public SystemCriticality Criticality { get; set; } = SystemCriticality.Medium;
    
    /// <summary>
    /// System version or release information
    /// </summary>
    public string? Version { get; set; }
    
    /// <summary>
    /// Vendor or provider information
    /// </summary>
    public string? Vendor { get; set; }
    
    /// <summary>
    /// Annual cost for budget tracking
    /// </summary>
    public decimal? AnnualCost { get; set; }
    
    /// <summary>
    /// Installation or go-live date
    /// </summary>
    public DateTime? InstallationDate { get; set; }
    
    /// <summary>
    /// Last maintenance or update date
    /// </summary>
    public DateTime? LastMaintenanceDate { get; set; }
    
    /// <summary>
    /// Planned retirement or end-of-life date
    /// </summary>
    public DateTime? PlannedRetirementDate { get; set; }
    
    /// <summary>
    /// Compliance requirements (GDPR, SOX, HIPAA, etc.)
    /// </summary>
    public string? ComplianceRequirements { get; set; }
    
    /// <summary>
    /// Technology stack or platform information
    /// </summary>
    public string? TechnologyStack { get; set; }
    
    /// <summary>
    /// Environment information (Production, Staging, Development)
    /// </summary>
    public string? Environment { get; set; }
    
    /// <summary>
    /// Free-form tags for categorization and filtering
    /// </summary>
    public List<string>? Tags { get; set; }
    
    /// <summary>
    /// Additional notes or comments
    /// </summary>
    public string? Notes { get; set; }
    
    // Hierarchical relationships
    /// <summary>
    /// Parent system ID for hierarchical structure
    /// </summary>
    public int? ParentSystemId { get; set; }
    
    /// <summary>
    /// Navigation property to parent system
    /// </summary>
    public virtual System? ParentSystem { get; set; }
    
    /// <summary>
    /// Collection of child systems/subsystems
    /// </summary>
    public virtual ICollection<System> SubSystems { get; set; } = new List<System>();
    
    // Dynamic attributes
    /// <summary>
    /// Collection of dynamic attributes for flexible metadata
    /// </summary>
    public virtual ICollection<SystemAttribute> Attributes { get; set; } = new List<SystemAttribute>();
    
    // Ownership and roles
    /// <summary>
    /// Collection of system owners and their roles
    /// </summary>
    public virtual ICollection<SystemOwnership> Ownerships { get; set; } = new List<SystemOwnership>();
    
    // Dependencies
    /// <summary>
    /// Systems that this system depends on (outgoing dependencies)
    /// </summary>
    public virtual ICollection<SystemDependency> OutgoingDependencies { get; set; } = new List<SystemDependency>();
    
    /// <summary>
    /// Systems that depend on this system (incoming dependencies)
    /// </summary>
    public virtual ICollection<SystemDependency> IncomingDependencies { get; set; } = new List<SystemDependency>();
    
    // Capabilities
    /// <summary>
    /// Business capabilities supported by this system
    /// </summary>
    public virtual ICollection<SystemCapability> SystemCapabilities { get; set; } = new List<SystemCapability>();
}