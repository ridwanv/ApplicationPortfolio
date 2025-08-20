// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class AttributeValueConfiguration : IEntityTypeConfiguration<AttributeValue>
{
    public void Configure(EntityTypeBuilder<AttributeValue> builder)
    {
        builder.ToTable("AttributeValues");
        builder.Ignore(e => e.DomainEvents);
        builder.HasIndex(x => new { x.ApplicationSystemId, x.AttributeDefinitionId }).IsUnique();

        builder
            .HasOne(x => x.ApplicationSystem)
            .WithMany()
            .HasForeignKey(x => x.ApplicationSystemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(x => x.AttributeDefinition)
            .WithMany()
            .HasForeignKey(x => x.AttributeDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

