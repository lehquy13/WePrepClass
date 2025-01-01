using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Domain.WePrepClassAggregates.Notifications;

public class Notification : AuditedAggregateRoot<Guid>
{
    public string Message { get; private set; } = null!;
    public string ObjectId { get; private set; } = null!;

    public bool IsRead { get; private set; }
    public Guid? To { get; private set; }

    public NotificationEventType NotificationEventType { get; private set; }

    private Notification()
    {
    }

    public static Notification Create(
        string message,
        string objectId,
        Guid? to,
        NotificationEventType notificationEventType)
        => new()
        {
            Message = message,
            ObjectId = objectId,
            To = to,
            NotificationEventType = notificationEventType
        };

    public void MarkAsRead() => IsRead = true;
}

public static class NotificationRecipientConstants
{
    public static readonly Guid? Center = null;
}