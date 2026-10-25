// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Enums;

namespace CleanArchitecture.Blazor.Application.Features.EstateDocuments.Specifications;

public class EstateDocumentAdvancedFilter : PaginationFilter
{
    public string? Name { get; set; }
    public EstateDocumentType? Type { get; set; }
    public EstateDocumentStatus? Status { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public DateTime? CompletedFrom { get; set; }
    public DateTime? CompletedTo { get; set; }
    public EstateDocumentListView ListView { get; set; } = EstateDocumentListView.All;
    public UserProfile? CurrentUser { get; set; }
}