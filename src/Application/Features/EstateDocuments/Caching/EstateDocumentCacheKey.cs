// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.EstateDocuments.Caching;

public static class EstateDocumentCacheKey
{
    public const string GetAllCacheKey = "all-EstateDocuments";
    public static string GetEstateDocumentByIdCacheKey(int id)
    {
        return $"GetEstateDocumentById,{id}";
    }
    public static string GetPaginationCacheKey(string parameters)
    {
        return $"EstateDocumentsWithPaginationQuery,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "estatedocument" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }
}