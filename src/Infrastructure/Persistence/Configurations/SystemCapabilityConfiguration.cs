// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class SystemCapabilityConfiguration : IEntityTypeConfiguration<SystemCapability>
{
    public void Configure(EntityTypeBuilder<SystemCapability> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.ImplementationLevel).HasMaxLength(50);
        builder.Property(t => t.Notes).HasMaxLength(1000);
        builder.Property(t => t.ResponsiblePerson).HasMaxLength(100);

        // Foreign key relationships
        builder.HasOne(t => t.System)
               .WithMany(t => t.SystemCapabilities)
               .HasForeignKey(t => t.SystemId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Capability)
               .WithMany(t => t.SystemCapabilities)
               .HasForeignKey(t => t.CapabilityId)
               .OnDelete(DeleteBehavior.Cascade);

        // Unique constraint to prevent duplicate system-capability mappings
        builder.HasIndex(t => new { t.SystemId, t.CapabilityId })
               .IsUnique();

        builder.Ignore(e => e.DomainEvents);
    }
}