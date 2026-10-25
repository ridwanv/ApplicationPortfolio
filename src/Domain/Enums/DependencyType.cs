// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Enums;

/// <summary>
/// Defines the type of dependency relationship between systems
/// </summary>
public enum DependencyType
{
    /// <summary>
    /// Direct API or service call dependency
    /// </summary>
    ApiCall = 0,
    
    /// <summary>
    /// Database connection or data access
    /// </summary>
    Database = 1,
    
    /// <summary>
    /// File system or shared storage dependency
    /// </summary>
    FileSystem = 2,
    
    /// <summary>
    /// Message queue or event-based dependency
    /// </summary>
    MessageQueue = 3,
    
    /// <summary>
    /// Network or infrastructure dependency
    /// </summary>
    Network = 4,
    
    /// <summary>
    /// Authentication or identity dependency
    /// </summary>
    Authentication = 5,
    
    /// <summary>
    /// Configuration or environment dependency
    /// </summary>
    Configuration = 6,
    
    /// <summary>
    /// Third-party service dependency
    /// </summary>
    ThirdParty = 7,
    
    /// <summary>
    /// Data synchronization dependency
    /// </summary>
    DataSync = 8,
    
    /// <summary>
    /// Business process dependency
    /// </summary>
    BusinessProcess = 9
}