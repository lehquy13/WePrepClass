using FluentAssertions;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Payments;

namespace WePrepClass.Domain.UnitTests;

public class PaymentUnitTests
{
    [Fact]
    public void Create_WhenAmountIsNegative_ShouldReturnError()
    {
        // Arrange
        const decimal amount = -1;
        const PaymentMethod paymentMethod = PaymentMethod.CreditCard;

        // Act
        var payment = Payment.Create(amount, paymentMethod);

        // Assert
        payment.IsFailed.Should().BeTrue();
        payment.Error.Code.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WhenAmountIsZero_ShouldReturnError()
    {
        // Arrange
        const decimal amount = 0;
        const PaymentMethod paymentMethod = PaymentMethod.CreditCard;

        // Act
        var payment = Payment.Create(amount, paymentMethod);

        // Assert
        payment.IsFailed.Should().BeTrue();
        payment.Error.Code.Should().NotBeEmpty();
    }

    [Fact]
    public void Create_WhenAmountIsPositive_ShouldCreatePayment()
    {
        // Arrange
        const decimal amount = 100;
        const PaymentMethod paymentMethod = PaymentMethod.CreditCard;

        // Act
        var payment = Payment.Create(amount, paymentMethod);

        // Assert
        payment.IsSuccess.Should().BeTrue();

        payment.Value.Amount.Should().Be(amount);
        payment.Value.PaymentMethod.Should().Be(paymentMethod);
    }
}