// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CleanArchitecture.Blazor.Domain.Enums;

public enum EstateDocumentStatus
{
    Draft = 1,
    InProgress = 2,
    ReviewRequired = 3,
    Completed = 4,
    Signed = 5,
    Archived = 6
}