using Matt.SharedKernel.Domain.Interfaces.Repositories;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors;

public interface ITutorRepository : IRepository
{
    Task<List<Tutor>> GetPopularTutors();
    Task<Tutor?> GetTutorByUserId(UserId userId, CancellationToken cancellationToken = default);
    Task<List<TutorId>> GetTutorsBySubjectId(SubjectId create, CancellationToken cancellationToken = default);
}