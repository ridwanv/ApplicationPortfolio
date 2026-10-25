// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.EstateDocuments.Specifications;

#nullable disable warnings
public class EstateDocumentAdvancedSpecification : Specification<EstateDocument>
{
    public EstateDocumentAdvancedSpecification(EstateDocumentAdvancedFilter filter)
    {
        DateTime today = DateTime.UtcNow;
        var todayrange = today.GetDateRange("TODAY", filter.CurrentUser.LocalTimeOffset);
        var last30daysrange = today.GetDateRange("LAST_30_DAYS", filter.CurrentUser.LocalTimeOffset);
        
        Query.Where(x => x.Name != null)
            .Where(x => x.Name.Contains(filter.Keyword) || x.Content.Contains(filter.Keyword), !string.IsNullOrEmpty(filter.Keyword))
            .Where(x => x.Name.Contains(filter.Name), !string.IsNullOrEmpty(filter.Name))
            .Where(x => x.Type == filter.Type, filter.Type.HasValue)
            .Where(x => x.Status == filter.Status, filter.Status.HasValue)
            .Where(x => x.Price <= filter.MaxPrice, filter.MaxPrice is not null)
            .Where(x => x.Price >= filter.MinPrice, filter.MinPrice is not null)
            .Where(x => x.CompletedDate >= filter.CompletedFrom, filter.CompletedFrom.HasValue)
            .Where(x => x.CompletedDate <= filter.CompletedTo, filter.CompletedTo.HasValue)
            .Where(x => x.CreatedBy == filter.CurrentUser.UserId, filter.ListView == EstateDocumentListView.My)
            .Where(x => x.Created >= todayrange.Start && x.Created < todayrange.End.AddDays(1), filter.ListView == EstateDocumentListView.Created_Today)
            .Where(x => x.Created >= last30daysrange.Start, filter.ListView == EstateDocumentListView.Created_30days);
    }
}