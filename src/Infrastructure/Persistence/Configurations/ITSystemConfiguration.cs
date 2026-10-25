// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class ITSystemConfiguration : IEntityTypeConfiguration<ITSystem>
{
    public void Configure(EntityTypeBuilder<ITSystem> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.Owner).HasMaxLength(100);
        builder.Property(t => t.Version).HasMaxLength(50);
        builder.Property(t => t.Status).HasMaxLength(50);
        builder.Property(t => t.Category).HasMaxLength(100);
        builder.Property(t => t.Vendor).HasMaxLength(100);
        builder.Property(t => t.CriticalityLevel).HasMaxLength(50);
        builder.Property(t => t.ComplianceRequirements).HasMaxLength(1000);
        builder.Property(t => t.MaintenanceNotes).HasMaxLength(1000);
        builder.Property(t => t.AnnualCost).HasColumnType("decimal(18,2)");

        // Self-referencing relationship for parent/child systems
        builder.HasOne(t => t.ParentSystem)
               .WithMany(t => t.SubSystems)
               .HasForeignKey(t => t.ParentSystemId)
               .OnDelete(DeleteBehavior.Restrict);

        // Many-to-many relationship with capabilities through SystemCapability
        builder.HasMany(t => t.SystemCapabilities)
               .WithOne(t => t.ITSystem)
               .HasForeignKey(t => t.ITSystemId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.DomainEvents);
    }
}