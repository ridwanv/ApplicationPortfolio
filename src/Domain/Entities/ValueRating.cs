// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public class ValueRating : BaseAuditableEntity
{
    public int ApplicationSystemId { get; set; }
    public ApplicationSystem? ApplicationSystem { get; set; }
    public int Score { get; set; } // 1-5
}

