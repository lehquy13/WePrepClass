using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;

namespace WePrepClass.Application.UseCases.Administrator.Subjects.Queries;

public record GetAllSubjectsQuery : IQueryRequest<List<SubjectDto>>;

public class GetAllSubjectsQueryHandler(
    ISubjectRepository subjectRepository,
    IAppLogger<GetAllSubjectsQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetAllSubjectsQuery, List<SubjectDto>>(logger, mapper)
{
    public override async Task<Result<List<SubjectDto>>> Handle(GetAllSubjectsQuery getAllUserQuery,
        CancellationToken cancellationToken)
    {
        var subjects = await subjectRepository.GetAllListAsync(cancellationToken);

        var subjectDtos = Mapper.Map<List<SubjectDto>>(subjects);
        return subjectDtos;
    }
}