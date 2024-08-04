using FluentValidation;
using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;

namespace WePrepClass.Application.UseCases.Administrator.Subjects.Queries;

public record GetSubjectQuery(int Id) : IQueryRequest<SubjectDto>;

public class GetSubjectQueryValidator : AbstractValidator<GetSubjectQuery>
{
    public GetSubjectQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public class GetSubjectQueryHandler(
    ISubjectRepository subjectRepository,
    IAppLogger<GetSubjectQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetSubjectQuery, SubjectDto>(logger, mapper)
{
    public override async Task<Result<SubjectDto>> Handle(GetSubjectQuery getAllUserQuery,
        CancellationToken cancellationToken)
    {
        var subjects = await subjectRepository.GetAsync(SubjectId.Create(getAllUserQuery.Id), cancellationToken);

        if (subjects is null) return Result.Fail(AppServiceError.Subject.NotExist);

        var subjectDtos = Mapper.Map<SubjectDto>(subjects);

        return subjectDtos;
    }
}