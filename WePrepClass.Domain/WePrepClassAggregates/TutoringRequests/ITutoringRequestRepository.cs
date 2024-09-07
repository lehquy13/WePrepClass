using Matt.SharedKernel.Domain.Interfaces.Repositories;

namespace WePrepClass.Domain.WePrepClassAggregates.TutoringRequests;

public interface ITutoringRequestRepository : IRepository
{
    Task Insert(TutoringRequest tutoringRequest);
}