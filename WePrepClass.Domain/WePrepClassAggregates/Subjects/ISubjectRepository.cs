using Matt.SharedKernel.Domain.Interfaces.Repositories;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Subjects;

public interface ISubjectRepository : IRepository
{
    Task<List<Subject>> GetListByIdsAsync(IEnumerable<SubjectId> subjectIds, CancellationToken cancellationToken = default);
    Task<List<Subject>> GetAllListAsync(CancellationToken cancellationToken = default);
    Task<Subject?> GetAsync(SubjectId subjectId, CancellationToken cancellationToken = default);
    Task InsertAsync(Subject subject);
}