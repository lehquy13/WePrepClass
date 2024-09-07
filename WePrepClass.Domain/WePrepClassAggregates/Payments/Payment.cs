using Matt.ResultObject;
using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Payments.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Payments;

public class Payment : AggregateRoot<PaymentId>
{
    public decimal Amount { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.Paid;
    public PaymentMethod PaymentMethod { get; private set; } = PaymentMethod.CreditCard;

    private Payment()
    {
    }

    public static Result<Payment> Create(decimal amount, PaymentMethod paymentMethod)
    {
        if (amount <= 0)
        {
            return Result.Fail(DomainErrors.Payments.AmountMustBeGreaterThanZero);
        }

        return new Payment
        {
            Id = PaymentId.Create(),
            Amount = amount,
            PaymentMethod = paymentMethod
        };
    }
}