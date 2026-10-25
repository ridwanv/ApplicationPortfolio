// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class EstateAsset : BaseAuditableEntity
{
    public int EstateDocumentId { get; set; }
    public string? Name { get; set; }
    public AssetType Type { get; set; }
    public decimal EstimatedValue { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? AccountNumber { get; set; }
    public string? Institution { get; set; }
    public bool IsSharedAsset { get; set; }
    
    // Navigation property
    public virtual EstateDocument EstateDocument { get; set; } = null!;
}