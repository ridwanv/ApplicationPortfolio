// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.SystemDependencies.Caching;

public static class SystemDependencyCacheKey
{
    public const string GetAllCacheKey = "all-SystemDependencies";
    public static string GetBySystemIdCacheKey(int systemId) => $"GetDependenciesBySystemId,{systemId}";
    public static string GetDependencyGraphCacheKey(int? rootSystemId) => $"GetDependencyGraph,RootId:{rootSystemId}";
    public static string GetImpactAnalysisCacheKey(int systemId) => $"GetImpactAnalysis,SystemId:{systemId}";
    public static IEnumerable<string>? Tags => new string[] { "systemdependency", "orgsystem" };
    public static void Refresh() => FusionCacheFactory.RemoveByTags(Tags);
}