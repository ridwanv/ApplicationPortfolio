// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.AttributeDefinitions.Caching;

public static class AttributeDefinitionCacheKey
{
    public const string GetAllCacheKey = "all-AttributeDefinitions";
    public static string GetByIdCacheKey(int id) => $"GetAttributeDefinitionById,{id}";
    public static string GetByNameCacheKey(string name) => $"GetAttributeDefinitionByName,{name}";
    public static string GetPaginationCacheKey(string parameters) => $"AttributeDefinitionsWithPaginationQuery,{parameters}";
    public static IEnumerable<string>? Tags => new string[] { "attributedefinition" };
    public static void Refresh() => FusionCacheFactory.RemoveByTags(Tags);
}