using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Domain.WePrepClassAggregates.Notifications;

public class Notification : AuditedAggregateRoot<int>
{
    public string Message { get; private set; } = null!;
    public string ObjectId { get; private set; } = null!;
    public bool IsRead { get; private set; }
    public NotificationEventType NotificationEventType { get; private set; }

    private Notification()
    {
    }

    public static Notification Create(
        string message,
        string objectId,
        NotificationEventType notificationEventType)
    {
        return new Notification
        {
            Message = message,
            ObjectId = objectId,
            NotificationEventType = notificationEventType
        };
    }
}