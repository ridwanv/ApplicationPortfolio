// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Domain.Entities;

/// <summary>
/// Represents dependency relationships between systems for mapping and visualization
/// </summary>
public class SystemDependency : BaseAuditableEntity
{
    /// <summary>
    /// Source system that depends on the target system
    /// </summary>
    public int SourceSystemId { get; set; }
    
    /// <summary>
    /// Target system that is depended upon
    /// </summary>
    public int TargetSystemId { get; set; }
    
    /// <summary>
    /// Type of dependency relationship
    /// </summary>
    public DependencyType DependencyType { get; set; }
    
    /// <summary>
    /// Criticality of this dependency
    /// </summary>
    public SystemCriticality Criticality { get; set; } = SystemCriticality.Medium;
    
    /// <summary>
    /// Description of the dependency relationship
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Technical details about the dependency (endpoints, protocols, etc.)
    /// </summary>
    public string? TechnicalDetails { get; set; }
    
    /// <summary>
    /// Service Level Agreement or performance requirements
    /// </summary>
    public string? SLA { get; set; }
    
    /// <summary>
    /// Whether this dependency is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Date when this dependency was established
    /// </summary>
    public DateTime? EstablishedDate { get; set; }
    
    /// <summary>
    /// Planned removal date (for dependencies being phased out)
    /// </summary>
    public DateTime? PlannedRemovalDate { get; set; }
    
    /// <summary>
    /// Impact assessment if this dependency fails
    /// </summary>
    public string? FailureImpact { get; set; }
    
    /// <summary>
    /// Backup or alternative solutions if this dependency is unavailable
    /// </summary>
    public string? AlternativeSolution { get; set; }
    
    /// <summary>
    /// Additional notes about the dependency
    /// </summary>
    public string? Notes { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Navigation property to the source system
    /// </summary>
    public virtual System SourceSystem { get; set; } = default!;
    
    /// <summary>
    /// Navigation property to the target system
    /// </summary>
    public virtual System TargetSystem { get; set; } = default!;
}