// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Attributes.Caching;

public static class AttributeCacheKey
{
    public const string GetAllDefinitionsCacheKey = "all-AttributeDefinitions";
    public static string GetDefinitionByIdCacheKey(int id) => $"GetAttributeDefinitionById,{id}";
    public static string GetValuesBySystemCacheKey(int systemId) => $"GetAttributeValuesBySystem,{systemId}";
    public static IEnumerable<string>? Tags => new[] { "attributes" };
    public static void Refresh() => FusionCacheFactory.RemoveByTags(Tags);
}

