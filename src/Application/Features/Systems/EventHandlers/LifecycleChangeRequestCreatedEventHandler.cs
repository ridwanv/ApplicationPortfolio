// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using CleanArchitecture.Blazor.Server.UI.Services.Notifications;

namespace CleanArchitecture.Blazor.Application.Features.Systems.EventHandlers;

public class LifecycleChangeRequestCreatedEventHandler : INotificationHandler<LifecycleChangeRequestCreatedEvent>
{
    private readonly ILogger<LifecycleChangeRequestCreatedEventHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public LifecycleChangeRequestCreatedEventHandler(ILogger<LifecycleChangeRequestCreatedEventHandler> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task Handle(LifecycleChangeRequestCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var notifier = scope.ServiceProvider.GetRequiredService<INotificationService>();
            await notifier.AddNotification(new NotificationMessage(
                Guid.NewGuid().ToString(),
                "Lifecycle change request submitted",
                $"Request {notification.RequestId} submitted for system {notification.SystemId} to stage {notification.ToStage}",
                "Workflow",
                DateTime.UtcNow,
                string.Empty,
                new[] { new NotificationAuthor("System", "") },
                typeof(NotificationMessage)
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification for lifecycle request {RequestId}", notification.RequestId);
        }
    }
}

