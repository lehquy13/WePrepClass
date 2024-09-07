using FluentAssertions;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Payments.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subscriptions;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.UnitTests;

public class SubscriptionUnitTests
{
    [Fact]
    public void Create_WhenValid_ShouldCreateSubscription()
    {
        // Arrange
        var userId = UserId.Create();
        const SubscriptionPackage subscriptionPackage = SubscriptionPackage.Standard;
        const SubscriptionType subscriptionEnums = SubscriptionType.Monthly;

        // Act
        var subscription = Subscription.Create(userId, subscriptionEnums, subscriptionPackage);

        // Assert
        subscription.Should().NotBeNull();

        subscription.UserId.Should().Be(userId);
        subscription.SubscriptionPackage.Should().Be(subscriptionPackage);
        subscription.SubscriptionType.Should().Be(subscriptionEnums);
    }

    [Fact]
    public void ActivateSubscription_WhenValid_ShouldActivateSubscription()
    {
        // Arrange
        var userId = UserId.Create();
        const SubscriptionPackage subscriptionPackage = SubscriptionPackage.Standard;
        const SubscriptionType subscriptionEnums = SubscriptionType.Monthly;
        var subscription = Subscription.Create(userId, subscriptionEnums, subscriptionPackage);

        var paymentId = PaymentId.Create();

        // Act
        subscription.ActivateSubscription(paymentId);

        // Assert
        subscription.PaymentId.Should().Be(paymentId);
        subscription.SubscriptionStatus.Should().Be(SubscriptionStatus.Active);
    }
}