// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class ValueRatingConfiguration : IEntityTypeConfiguration<ValueRating>
{
    public void Configure(EntityTypeBuilder<ValueRating> builder)
    {
        builder.ToTable("ValueRatings");
        builder.Ignore(e => e.DomainEvents);
        builder.HasIndex(x => x.ApplicationSystemId).IsUnique();
        builder.Property(x => x.Score).HasDefaultValue(3);
        builder.HasOne(x => x.ApplicationSystem)
            .WithMany()
            .HasForeignKey(x => x.ApplicationSystemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

