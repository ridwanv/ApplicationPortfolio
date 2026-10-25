// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class SystemDependencyConfiguration : IEntityTypeConfiguration<SystemDependency>
{
    public void Configure(EntityTypeBuilder<SystemDependency> builder)
    {
        // Properties
        builder.Property(x => x.Description)
            .HasMaxLength(1000);
        
        builder.Property(x => x.TechnicalDetails)
            .HasMaxLength(2000);
        
        builder.Property(x => x.SLA)
            .HasMaxLength(500);
        
        builder.Property(x => x.FailureImpact)
            .HasMaxLength(1000);
        
        builder.Property(x => x.AlternativeSolution)
            .HasMaxLength(1000);
        
        builder.Property(x => x.Notes)
            .HasMaxLength(2000);
        
        // Relationships
        builder.HasOne(x => x.SourceSystem)
            .WithMany(s => s.OutgoingDependencies)
            .HasForeignKey(x => x.SourceSystemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.TargetSystem)
            .WithMany(s => s.IncomingDependencies)
            .HasForeignKey(x => x.TargetSystemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(x => new { x.SourceSystemId, x.TargetSystemId })
            .HasDatabaseName("IX_SystemDependency_Source_Target");
        
        builder.HasIndex(x => x.DependencyType);
        builder.HasIndex(x => x.Criticality);
        builder.HasIndex(x => x.IsActive);
        
        // Prevent self-dependencies
        builder.HasCheckConstraint("CK_SystemDependency_NoSelfReference", 
            "[SourceSystemId] != [TargetSystemId]");
        
        builder.Ignore(e => e.DomainEvents);
    }
}