// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common.Entities;

namespace CleanArchitecture.Blazor.Domain.Entities;

public enum LifecycleRequestStatus { Draft, Submitted, UnderReview, Approved, Rejected }

public class LifecycleChangeRequest : BaseAuditableEntity
{
    public int ApplicationSystemId { get; set; }
    public ApplicationSystem? ApplicationSystem { get; set; }
    public string? FromStage { get; set; }
    public string? ToStage { get; set; }
    public string? RequestedBy { get; set; }
    public string? Comments { get; set; }
    public LifecycleRequestStatus Status { get; set; } = LifecycleRequestStatus.Draft;
}

