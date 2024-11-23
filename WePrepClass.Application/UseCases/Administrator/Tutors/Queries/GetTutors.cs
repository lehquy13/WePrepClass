using MapsterMapper;
using Matt.Paginated;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Tutors;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Users;

namespace WePrepClass.Application.UseCases.Administrator.Tutors.Queries;

public record GetTutorsQuery(
    GetTutorsRequest TutorParams
) : IQueryRequest<PaginatedList<TutorListDto>>, IAuthorizationRequired;

public class GetTutorsQueryHandler(
    IReadDbContext dbContext,
    IAppLogger<GetTutorsQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetTutorsQuery, PaginatedList<TutorListDto>>(logger, mapper)
{
    public override async Task<Result<PaginatedList<TutorListDto>>> Handle(GetTutorsQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<(Tutor Tutor, IEnumerable<Subject> Majors, User User)>
            tutors =
                from tutor in dbContext.Tutors
                join major in dbContext.Majors on tutor.Id equals major.TutorId
                join subject in dbContext.Subjects on major.SubjectId equals subject.Id into majors
                join user in dbContext.Users on tutor.UserId equals user.Id
                select new ValueTuple<Tutor, IEnumerable<Subject>, User>(
                    tutor,
                    majors,
                    user
                );

        var totalCount = await tutors.LongCountAsync(cancellationToken);

        var queryResults = await tutors
            .Skip((request.TutorParams.PageIndex - 1) * request.TutorParams.PageSize)
            .Take(request.TutorParams.PageSize)
            .ToListAsync(cancellationToken);

        var tutorListDtos = queryResults.Select(
            x => new TutorListDto
            {
                Id = x.Tutor.Id.Value,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                BirthYear = x.User.BirthYear,
                Description = x.User.Description,
                Avatar = x.User.Avatar,
                AcademicLevel = x.Tutor.AcademicLevel.ToString(),
                University = x.Tutor.University,
                Rate = x.Tutor.Rate
            }
        );

        var result = PaginatedList<TutorListDto>
            .Create(
                tutorListDtos,
                request.TutorParams.PageIndex,
                request.TutorParams.PageSize,
                (int)totalCount);

        return result;
    }
}