// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Domain.Common;

namespace CleanArchitecture.Blazor.Domain.Events;

public class EstateDocumentDeletedEvent : DomainEvent
{
    public EstateDocumentDeletedEvent(Entities.EstateDocument item)
    {
        Item = item;
    }

    public Entities.EstateDocument Item { get; }
}