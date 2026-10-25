// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class SystemAttributeConfiguration : IEntityTypeConfiguration<SystemAttribute>
{
    public void Configure(EntityTypeBuilder<SystemAttribute> builder)
    {
        builder.Property(x => x.Value).HasMaxLength(2048);
        builder.HasIndex(x => new { x.SystemId, x.AttributeDefinitionId }).IsUnique();
        builder.HasOne(x => x.System)
            .WithMany(s => s.Attributes)
            .HasForeignKey(x => x.SystemId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.AttributeDefinition)
            .WithMany()
            .HasForeignKey(x => x.AttributeDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Ignore(e => e.DomainEvents);
    }
}


