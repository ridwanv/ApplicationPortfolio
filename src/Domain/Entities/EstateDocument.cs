// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class EstateDocument : BaseAuditableEntity
{
    public string? Name { get; set; }
    public EstateDocumentType Type { get; set; }
    public EstateDocumentStatus Status { get; set; }
    public string? Content { get; set; }
    public string? TemplateData { get; set; }
    public decimal Price { get; set; }
    public DateTime? CompletedDate { get; set; }
    
    // Navigation properties
    public virtual ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();
    public virtual ICollection<EstateAsset> Assets { get; set; } = new List<EstateAsset>();
}