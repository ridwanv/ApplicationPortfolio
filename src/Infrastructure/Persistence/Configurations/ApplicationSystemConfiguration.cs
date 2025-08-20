// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class ApplicationSystemConfiguration : IEntityTypeConfiguration<ApplicationSystem>
{
    public void Configure(EntityTypeBuilder<ApplicationSystem> builder)
    {
        builder.ToTable("ApplicationSystems");
        builder.Property(x => x.Name).HasMaxLength(180).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(80);
        builder.Property(x => x.Hosting).HasMaxLength(80);
        builder.Property(x => x.LifecycleStage).HasMaxLength(80);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Ignore(e => e.DomainEvents);

        builder
            .HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

