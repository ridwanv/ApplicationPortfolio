// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Enums;
using CleanArchitecture.Blazor.Domain.Identity;

namespace CleanArchitecture.Blazor.Domain.Entities;

/// <summary>
/// Represents ownership and role assignments for systems in APM
/// </summary>
public class SystemOwnership : BaseAuditableEntity
{
    /// <summary>
    /// Reference to the system
    /// </summary>
    public int SystemId { get; set; }
    
    /// <summary>
    /// Reference to the user/owner
    /// </summary>
    public string UserId { get; set; } = string.Empty;
    
    /// <summary>
    /// Role of the user for this system
    /// </summary>
    public SystemRole Role { get; set; }
    
    /// <summary>
    /// Start date of ownership/role assignment
    /// </summary>
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// End date of ownership/role assignment (null = current)
    /// </summary>
    public DateTime? EndDate { get; set; }
    
    /// <summary>
    /// Whether this ownership record is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Additional notes about the ownership/role
    /// </summary>
    public string? Notes { get; set; }
    
    // Navigation properties
    /// <summary>
    /// Navigation property to the system
    /// </summary>
    public virtual System System { get; set; } = default!;
    
    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser User { get; set; } = default!;
}