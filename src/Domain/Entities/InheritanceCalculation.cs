// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class InheritanceCalculation : BaseAuditableEntity
{
    public int EstateDocumentId { get; set; }
    public decimal TotalEstateValue { get; set; }
    public decimal DebtAmount { get; set; }
    public decimal NetEstateValue { get; set; }
    public string? CalculationData { get; set; } // JSON data for calculation details
    public DateTime CalculatedDate { get; set; }
    
    // Navigation property
    public virtual EstateDocument EstateDocument { get; set; } = null!;
}