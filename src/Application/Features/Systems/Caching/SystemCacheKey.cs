// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Application.Features.Systems.Caching;

public static class SystemCacheKey
{
    public const string GetAllCacheKey = "all-Systems";
    public static string GetByIdCacheKey(int id) => $"GetSystemById,{id}";
    public static string GetPaginationCacheKey(string parameters) => $"SystemsWithPaginationQuery,{parameters}";
    public static IEnumerable<string>? Tags => new[] { "systems" };
    public static void Refresh() => FusionCacheFactory.RemoveByTags(Tags);
}

