// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Enums;

/// <summary>
/// Defines the type of system in the Application Portfolio Management catalog
/// </summary>
public enum SystemType
{
    /// <summary>
    /// Core business application system
    /// </summary>
    Application = 0,
    
    /// <summary>
    /// Infrastructure system (databases, middleware, etc.)
    /// </summary>
    Infrastructure = 1,
    
    /// <summary>
    /// Platform service (APIs, shared services)
    /// </summary>
    Platform = 2,
    
    /// <summary>
    /// Integration system (ESB, API gateway, etc.)
    /// </summary>
    Integration = 3,
    
    /// <summary>
    /// Data system (data warehouse, analytics)
    /// </summary>
    Data = 4,
    
    /// <summary>
    /// Security system (identity, access management)
    /// </summary>
    Security = 5,
    
    /// <summary>
    /// Monitoring and observability system
    /// </summary>
    Monitoring = 6,
    
    /// <summary>
    /// Development and deployment tooling
    /// </summary>
    DevOps = 7
}