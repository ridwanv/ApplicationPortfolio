// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using CleanArchitecture.Blazor.Domain.Identity;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CleanArchitecture.Blazor.Application.Common.Interfaces;

public interface IApplicationDbContext: IAsyncDisposable
{
    DbSet<SystemLog> SystemLogs { get; set; }
    DbSet<AuditTrail> AuditTrails { get; set; }
    DbSet<Document> Documents { get; set; }
    DbSet<PicklistSet> PicklistSets { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Tenant> Tenants { get; set; }
    DbSet<Contact> Contacts { get; set; }
    DbSet<ITSystem> ITSystems { get; set; }
    DbSet<OrgSystem> OrgSystems { get; set; }
    DbSet<Capability> Capabilities { get; set; }
    DbSet<SystemCapability> SystemCapabilities { get; set; }
    DbSet<AttributeDefinition> AttributeDefinitions { get; set; }
    DbSet<SystemAttribute> SystemAttributes { get; set; }
    DbSet<DependencyEdge> DependencyEdges { get; set; }
    
    // APM System entities
    DbSet<System> Systems { get; set; }
    DbSet<SystemOwnership> SystemOwnerships { get; set; }
    DbSet<SystemDependency> SystemDependencies { get; set; }
    
    DbSet<LoginAudit> LoginAudits { get; set; }
    DbSet<UserLoginRiskSummary> UserLoginRiskSummaries { get; set; }
    ChangeTracker ChangeTracker { get; }

    DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}