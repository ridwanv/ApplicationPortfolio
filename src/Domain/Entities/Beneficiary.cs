// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class Beneficiary : BaseAuditableEntity
{
    public int EstateDocumentId { get; set; }
    public string? Name { get; set; }
    public string? Relationship { get; set; }
    public BeneficiaryType Type { get; set; }
    public decimal InheritancePercentage { get; set; }
    public string? ContactInfo { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public bool IsMinor { get; set; }
    public string? GuardianName { get; set; }
    
    // Navigation property
    public virtual EstateDocument EstateDocument { get; set; } = null!;
}