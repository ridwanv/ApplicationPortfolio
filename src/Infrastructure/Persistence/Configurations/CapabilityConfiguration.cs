// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

#nullable disable
public class CapabilityConfiguration : IEntityTypeConfiguration<Capability>
{
    public void Configure(EntityTypeBuilder<Capability> builder)
    {
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.Category).HasMaxLength(100);
        builder.Property(t => t.Level).HasMaxLength(50);
        builder.Property(t => t.Owner).HasMaxLength(100);
        builder.Property(t => t.Status).HasMaxLength(50);
        builder.Property(t => t.MaturityLevel).HasMaxLength(50);
        builder.Property(t => t.BusinessValue).HasMaxLength(1000);
        builder.Property(t => t.Dependencies).HasMaxLength(1000);
        builder.Property(t => t.Risks).HasMaxLength(1000);

        // Many-to-many relationship with systems through SystemCapability
        builder.HasMany(t => t.SystemCapabilities)
               .WithOne(t => t.Capability)
               .HasForeignKey(t => t.CapabilityId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.DomainEvents);
    }
}