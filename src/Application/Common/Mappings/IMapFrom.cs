// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using AutoMapper;

namespace CleanArchitecture.Blazor.Application.Common.Mappings;

/// <summary>
/// Marker interface to indicate a type maps from <typeparamref name="T"/>.
/// Implementors may optionally provide a nested AutoMapper Profile or use
/// a separate mapping profile. This keeps generated DTOs compiling where
/// IMapFrom is referenced.
/// </summary>
/// <typeparam name="T">Source type to map from.</typeparam>
public interface IMapFrom<T>
{
}


