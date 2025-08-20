// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class LifecycleChangeRequestConfiguration : IEntityTypeConfiguration<LifecycleChangeRequest>
{
    public void Configure(EntityTypeBuilder<LifecycleChangeRequest> builder)
    {
        builder.ToTable("LifecycleChangeRequests");
        builder.Ignore(e => e.DomainEvents);
        builder.Property(x => x.FromStage).HasMaxLength(80);
        builder.Property(x => x.ToStage).HasMaxLength(80);
        builder.Property(x => x.RequestedBy).HasMaxLength(256);
        builder.Property(x => x.Comments).HasMaxLength(2048);
        builder.HasOne(x => x.ApplicationSystem)
            .WithMany()
            .HasForeignKey(x => x.ApplicationSystemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

