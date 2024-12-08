namespace WePrepClass.Domain.WePrepClassAggregates.Notifications;

public interface INotificationRepository
{
    void Insert(Notification notification);
}