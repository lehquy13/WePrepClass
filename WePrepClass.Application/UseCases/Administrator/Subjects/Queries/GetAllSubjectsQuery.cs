using MapsterMapper;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Results;
using WePrepClass.Contracts.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;

namespace WePrepClass.Application.UseCases.Administrator.Subjects.Queries;

public record GetAllSubjectsQuery : IQueryRequest<List<SubjectDto>>;

public class GetAllSubjectsQueryHandler(
    ISubjectRepository subjectRepository,
    IMapper mapper
) : QueryHandlerBase<GetAllSubjectsQuery, List<SubjectDto>>
{
    public override async Task<Result<List<SubjectDto>>> Handle(GetAllSubjectsQuery getAllUserQuery,
        CancellationToken cancellationToken)
    {
        var subjects = await subjectRepository.GetAllListAsync(cancellationToken);

        var subjectDtos = mapper.Map<List<SubjectDto>>(subjects);
        return subjectDtos;
    }
}