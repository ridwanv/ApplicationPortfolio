// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Enums;

/// <summary>
/// Defines user roles for system ownership and management in APM
/// </summary>
public enum SystemRole
{
    /// <summary>
    /// Chief Information Officer - enterprise oversight
    /// </summary>
    CIO = 0,
    
    /// <summary>
    /// Enterprise Architect - architectural governance
    /// </summary>
    Architect = 1,
    
    /// <summary>
    /// System Owner - responsible for system operations
    /// </summary>
    SystemOwner = 2,
    
    /// <summary>
    /// Business Owner - responsible for business requirements
    /// </summary>
    BusinessOwner = 3,
    
    /// <summary>
    /// Technical Owner - responsible for technical aspects
    /// </summary>
    TechnicalOwner = 4,
    
    /// <summary>
    /// Change Manager - manages system changes and releases
    /// </summary>
    ChangeManager = 5,
    
    /// <summary>
    /// Application Manager - day-to-day application management
    /// </summary>
    ApplicationManager = 6,
    
    /// <summary>
    /// Data Steward - responsible for data governance
    /// </summary>
    DataSteward = 7
}