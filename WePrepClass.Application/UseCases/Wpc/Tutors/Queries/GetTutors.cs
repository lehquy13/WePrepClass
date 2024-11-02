using MapsterMapper;
using Matt.Paginated;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Tutors;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Tutors.Queries;

public record GetTutorsQuery(
    GetTutorsRequest TutorParams
) : IQueryRequest<PaginatedList<TutorListDto>>;

public class GetTutorsQueryHandler(
    IReadDbContext dbContext,
    ICurrentUserService currentUserService,
    IAppLogger<GetTutorsQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetTutorsQuery, PaginatedList<TutorListDto>>(logger, mapper)
{
    public override async Task<Result<PaginatedList<TutorListDto>>> Handle(
        GetTutorsQuery request,
        CancellationToken cancellationToken
    )
    {
        var tutors =
            from tutor in dbContext.Tutors
            join major in dbContext.Majors on tutor.Id equals major.TutorId
            join subject in dbContext.Subjects on major.SubjectId equals subject.Id into majors
            join user in dbContext.Users on tutor.UserId equals user.Id
            join course in dbContext.Courses on tutor.Id equals course.TutorId into assignedCourses
            where tutor.TutorStatus == TutorStatus.Active
            select new
            {
                Tutor = tutor,
                Majors = majors,
                User = user,
                Courses = assignedCourses
            };

        if (request.TutorParams.Academic?.ToEnum<AcademicLevel>() is { } academicLevel &&
            academicLevel != AcademicLevel.Optional)
            tutors = tutors.Where(record =>
                record.User != null &&
                record.Tutor.AcademicLevel == academicLevel);

        if (!string.IsNullOrEmpty(request.TutorParams.City) &&
            !string.IsNullOrEmpty(request.TutorParams.District)
           )
            tutors = tutors.Where(record =>
                record.User.Address.Match(request.TutorParams.City, request.TutorParams.District));

        if (request.TutorParams.Gender is { } gender)
            tutors = tutors.Where(record => record.User.Gender == gender.ToEnum<Gender>());

        if (request.TutorParams.BirthYear != 0)
            tutors = tutors.Where(record => record.User.BirthYear == request.TutorParams.BirthYear);

        if (!string.IsNullOrEmpty(request.TutorParams.Subject))
            tutors = tutors.Where(record => record.Majors.Any(sub =>
                sub.Name.Contains(request.TutorParams.Subject, StringComparison.CurrentCultureIgnoreCase)));

        var totalCount = await dbContext.Tutors.LongCountAsync(cancellationToken);

        if (currentUserService.IsAuthenticated)
        {
            // TODO: include user's discoveries and learnt subjects into token or caching them
            IEnumerable<SubjectId> learntAndDiscoveriesSubjectIds = [];

            // Order by the number of subjects that the user has discovered
            tutors = tutors
                .OrderByDescending(record => record.Tutor.Rate)
                .ThenByDescending(record => record.Courses.Count())
                .ThenByDescending(record => learntAndDiscoveriesSubjectIds
                    .Count(id => record.Majors.Any(major => major.Id == id)));
        }
        else
        {
            tutors = tutors.OrderByDescending(record => record.Tutor.Rate)
                .ThenByDescending(record => record.Courses.Count());
        }

        var queryResults = await tutors
            .Skip((request.TutorParams.PageIndex - 1) * request.TutorParams.PageSize)
            .Take(request.TutorParams.PageSize)
            .ToListAsync(cancellationToken);

        var mergeList = queryResults.Select(
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
                mergeList,
                request.TutorParams.PageIndex,
                request.TutorParams.PageSize,
                (int)totalCount);

        return result;
    }
}