using Matt.SharedKernel.Domain.Interfaces.Repositories;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors;

public interface ITutorRepository : IRepository
{
    Task<Tutor?> GetById(TutorId tutorId, CancellationToken cancellationToken = default);
    void Insert(Tutor tutorValue);
}