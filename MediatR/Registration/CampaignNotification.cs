using MediatR;
using Microsoft.Extensions.Logging;

namespace Registration;

public record CampaignChangedNotification(Guid CampaignId) : INotification;

public class CampaignNotification(ILogger<CampaignNotification> logger) : INotificationHandler<CampaignChangedNotification>
{
    public static event EventHandler<CampaignChangedNotification>? CampaignChanged;

    public Task Handle(CampaignChangedNotification notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Campaign changed: {CampaignId}", notification.CampaignId);
        CampaignChanged?.Invoke(this, notification);
        return Task.CompletedTask;
    }
}
