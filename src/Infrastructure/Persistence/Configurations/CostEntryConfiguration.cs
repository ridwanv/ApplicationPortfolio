// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class CostEntryConfiguration : IEntityTypeConfiguration<CostEntry>
{
    public void Configure(EntityTypeBuilder<CostEntry> builder)
    {
        builder.ToTable("CostEntries");
        builder.Ignore(e => e.DomainEvents);
        builder.HasIndex(x => new { x.ApplicationSystemId, x.Year, x.Month }).IsUnique();
        builder.Property(x => x.Opex).HasPrecision(18, 2);
        builder.Property(x => x.Capex).HasPrecision(18, 2);
        builder.HasOne(x => x.ApplicationSystem)
            .WithMany()
            .HasForeignKey(x => x.ApplicationSystemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

