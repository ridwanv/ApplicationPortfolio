// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;
using CleanArchitecture.Blazor.Domain.Common.Enums;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class AttributeDefinition : BaseAuditableEntity
{
    public string? Name { get; set; }
    public AttributeDataType DataType { get; set; }
    public bool IsRequired { get; set; }

    // For dropdowns or constrained lists
    public List<string>? Options { get; set; }
}

