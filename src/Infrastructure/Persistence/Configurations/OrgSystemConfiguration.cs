// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class OrgSystemConfiguration : IEntityTypeConfiguration<OrgSystem>
{
    public void Configure(EntityTypeBuilder<OrgSystem> builder)
    {
        builder.Property(x => x.Name).HasMaxLength(256).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(2048);
        builder.Property(x => x.Dependencies).HasMaxLength(2048);
        builder.Property(x => x.Tags)
            .HasConversion(
                v => string.Join("|", v ?? new List<string>()),
                v => string.IsNullOrWhiteSpace(v) ? new List<string>() : v.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList());

        // Configure hierarchical relationship
        builder.HasOne(x => x.ParentSystem)
            .WithMany(x => x.SubSystems)
            .HasForeignKey(x => x.ParentSystemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.Criticality);
        builder.HasIndex(x => x.LifecycleStatus);
        builder.HasIndex(x => x.ParentSystemId);
        builder.Ignore(e => e.DomainEvents);
    }
}


