// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Blazor.Infrastructure.Persistence.Configurations;

public class SystemOwnershipConfiguration : IEntityTypeConfiguration<SystemOwnership>
{
    public void Configure(EntityTypeBuilder<SystemOwnership> builder)
    {
        // Properties
        builder.Property(x => x.UserId)
            .HasMaxLength(450)
            .IsRequired();
        
        builder.Property(x => x.Notes)
            .HasMaxLength(1000);
        
        // Relationships
        builder.HasOne(x => x.System)
            .WithMany(s => s.Ownerships)
            .HasForeignKey(x => x.SystemId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Indexes
        builder.HasIndex(x => new { x.SystemId, x.UserId, x.Role })
            .HasDatabaseName("IX_SystemOwnership_System_User_Role");
        
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.Role);
        
        // Ensure unique active ownership per role per system
        builder.HasIndex(x => new { x.SystemId, x.Role, x.IsActive })
            .HasFilter("[IsActive] = 1")
            .HasDatabaseName("IX_SystemOwnership_Unique_Active_Role");
        
        builder.Ignore(e => e.DomainEvents);
    }
}