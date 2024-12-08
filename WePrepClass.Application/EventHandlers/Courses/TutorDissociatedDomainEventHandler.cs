using MediatR;
using WePrepClass.Domain.WePrepClassAggregates.Courses;

namespace WePrepClass.Application.EventHandlers.Courses;

public record TutorDissociatedDomainEventHandler : INotificationHandler<TutorDissociatedDomainEvent>
{
    public Task Handle(TutorDissociatedDomainEvent notification, CancellationToken cancellationToken)
    {
        //throw new NotImplementedException();
        return Task.CompletedTask;
    }
}