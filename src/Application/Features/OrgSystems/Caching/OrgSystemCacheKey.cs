// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.OrgSystems.Caching;

public static class OrgSystemCacheKey
{
    public const string GetAllCacheKey = "all-OrgSystems";
    public static string GetByIdCacheKey(int id) => $"GetOrgSystemById,{id}";
    public static string GetPaginationCacheKey(string parameters) => $"OrgSystemsWithPaginationQuery,{parameters}";
    public static string GetHierarchyCacheKey(int? rootId, bool includeAttributes) => $"GetSystemHierarchy,RootId:{rootId},Attributes:{includeAttributes}";
    public static string GetByNameCacheKey(string name) => $"GetOrgSystemByName,{name}";
    public static string GetSubSystemsCacheKey(int parentId) => $"GetSubSystems,ParentId:{parentId}";
    public static string GetSystemPathCacheKey(int systemId) => $"GetSystemPath,SystemId:{systemId}";
    public static IEnumerable<string>? Tags => new string[] { "orgsystem" };
    public static void Refresh() => FusionCacheFactory.RemoveByTags(Tags);
}



