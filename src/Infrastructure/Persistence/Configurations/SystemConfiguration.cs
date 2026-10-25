// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class SystemConfiguration : IEntityTypeConfiguration<System>
{
    public void Configure(EntityTypeBuilder<System> builder)
    {
        // Basic properties
        builder.Property(x => x.Name)
            .HasMaxLength(256)
            .IsRequired();
        
        builder.Property(x => x.Description)
            .HasMaxLength(2048);
        
        builder.Property(x => x.Version)
            .HasMaxLength(50);
        
        builder.Property(x => x.Vendor)
            .HasMaxLength(256);
        
        builder.Property(x => x.ComplianceRequirements)
            .HasMaxLength(1000);
        
        builder.Property(x => x.TechnologyStack)
            .HasMaxLength(500);
        
        builder.Property(x => x.Environment)
            .HasMaxLength(100);
        
        builder.Property(x => x.Notes)
            .HasMaxLength(2048);
        
        builder.Property(x => x.AnnualCost)
            .HasColumnType("decimal(18,2)");
        
        // Convert Tags list to pipe-separated string for storage
        builder.Property(x => x.Tags)
            .HasConversion(
                v => string.Join("|", v ?? new List<string>()),
                v => string.IsNullOrWhiteSpace(v) ? new List<string>() : v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList());
        
        // Hierarchical relationship (self-referencing)
        builder.HasOne(x => x.ParentSystem)
            .WithMany(x => x.SubSystems)
            .HasForeignKey(x => x.ParentSystemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes for common queries
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasIndex(x => x.SystemType);
        builder.HasIndex(x => x.LifecycleStatus);
        builder.HasIndex(x => x.Criticality);
        builder.HasIndex(x => x.ParentSystemId);
        
        // Configure relationships
        builder.HasMany(x => x.Attributes)
            .WithOne(a => a.System)
            .HasForeignKey(a => a.SystemId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Ownerships)
            .WithOne(o => o.System)
            .HasForeignKey(o => o.SystemId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.OutgoingDependencies)
            .WithOne(d => d.SourceSystem)
            .HasForeignKey(d => d.SourceSystemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(x => x.IncomingDependencies)
            .WithOne(d => d.TargetSystem)
            .HasForeignKey(d => d.TargetSystemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(x => x.SystemCapabilities)
            .WithOne(sc => sc.System)
            .HasForeignKey(sc => sc.SystemId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Ignore(e => e.DomainEvents);
    }
}