// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class DependencyEdgeConfiguration : IEntityTypeConfiguration<DependencyEdge>
{
    public void Configure(EntityTypeBuilder<DependencyEdge> builder)
    {
        builder.Property(x => x.Notes).HasMaxLength(1024);
        builder.HasIndex(x => new { x.SourceSystemId, x.TargetSystemId }).IsUnique();
        builder.HasOne(x => x.SourceSystem)
            .WithMany(s => s.OutgoingDependencies)
            .HasForeignKey(x => x.SourceSystemId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TargetSystem)
            .WithMany(s => s.IncomingDependencies)
            .HasForeignKey(x => x.TargetSystemId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Ignore(e => e.DomainEvents);
    }
}


