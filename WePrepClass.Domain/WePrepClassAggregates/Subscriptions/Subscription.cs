using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Payments.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subscriptions.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Subscriptions;

public class Subscription : AggregateRoot<SubscriptionId>
{
    public SubscriptionType SubscriptionType { get; private set; } = SubscriptionType.Monthly;
    public SubscriptionPackage SubscriptionPackage { get; private set; } = SubscriptionPackage.Standard;
    public SubscriptionStatus SubscriptionStatus { get; private set; } = SubscriptionStatus.Inactive;

    public UserId UserId { get; private set; } = null!;

    public PaymentId? PaymentId { get; private set; }

    private Subscription()
    {
    }

    public static Subscription Create(
        UserId userId,
        SubscriptionType subscriptionType,
        SubscriptionPackage subscriptionPackage)
    {
        return new Subscription
        {
            Id = SubscriptionId.Create(),
            UserId = userId,
            SubscriptionType = subscriptionType,
            SubscriptionPackage = subscriptionPackage
        };
    }

    public void ActivateSubscription(PaymentId paymentId)
    {
        PaymentId = paymentId;
        SubscriptionStatus = SubscriptionStatus.Active;
    }
}