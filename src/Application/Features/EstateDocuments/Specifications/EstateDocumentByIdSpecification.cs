// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Entities;

namespace CleanArchitecture.Blazor.Application.Features.EstateDocuments.Specifications;

public class EstateDocumentByIdSpecification : Specification<EstateDocument>
{
    public EstateDocumentByIdSpecification(int id)
    {
        Query.Where(x => x.Id == id)
            .Include(x => x.Beneficiaries)
            .Include(x => x.Assets);
    }
}