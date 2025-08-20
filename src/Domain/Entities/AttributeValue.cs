// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class AttributeValue : BaseAuditableEntity
{
    public int ApplicationSystemId { get; set; }
    public ApplicationSystem? ApplicationSystem { get; set; }

    public int AttributeDefinitionId { get; set; }
    public AttributeDefinition? AttributeDefinition { get; set; }

    // EAV storage fields (one will be used depending on DataType)
    public string? ValueText { get; set; }
    public decimal? ValueNumber { get; set; }
    public bool? ValueBool { get; set; }
    public DateTime? ValueDate { get; set; }
}

