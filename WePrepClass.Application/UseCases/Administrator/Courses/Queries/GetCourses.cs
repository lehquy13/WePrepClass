using MapsterMapper;
using Matt.Paginated;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Courses;
using WePrepClass.Domain.Commons;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;

namespace WePrepClass.Application.UseCases.Administrator.Courses.Queries;

public record GetCoursesQuery(GetCourseRequest CourseParams) : IQueryRequest<PaginatedList<CourseListDto>>;

public class GetCoursesQueryHandler(
    IReadDbContext dbContext,
    ICurrentUserService currentUserService,
    IAppLogger<GetCoursesQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetCoursesQuery, PaginatedList<CourseListDto>>(logger, mapper)
{
    public override async Task<Result<PaginatedList<CourseListDto>>> Handle(GetCoursesQuery request,
        CancellationToken cancellationToken)
    {
        IQueryable<(Course course, Subject subject)> courseQuery =
            from course in dbContext.Courses.OrderByDescending(x => x.CreationTime)
            join subject in dbContext.Subjects on course.SubjectId equals subject.Id
            select new ValueTuple<Course, Subject>(course, subject);

        courseQuery = ApplySearching(request, courseQuery);

        var count = await courseQuery.CountAsync(cancellationToken);

        courseQuery = ApplyUserOrientedSearching(courseQuery);

        var queryResult = await courseQuery
            .Skip((request.CourseParams.PageIndex - 1) * request.CourseParams.PageSize)
            .Take(request.CourseParams.PageSize)
            .Select(x => new CourseListDto
            {
                Id = x.course.Id.Value,
                Title = x.course.Title,
                Status = x.course.Status.ToString(),
                LearningMode = x.course.LearningModeRequirement.ToString(),
                SubjectName = x.subject.Name
            })
            .ToListAsync(cancellationToken: cancellationToken);

        return PaginatedList<CourseListDto>.Create(
            queryResult, request.CourseParams.PageIndex, request.CourseParams.PageSize, count);
    }

    private static IQueryable<(Course course, Subject subject)> ApplySearching(GetCoursesQuery request,
        IQueryable<(Course course, Subject subject)> courseQuery)
    {
        if (!string.IsNullOrWhiteSpace(request.CourseParams.SubjectName))
        {
            courseQuery = courseQuery.Where(x =>
                x.subject.Name.Contains(request.CourseParams.SubjectName, StringComparison.CurrentCultureIgnoreCase));
        }

        return courseQuery;
    }

    private IQueryable<(Course course, Subject subject)> ApplyUserOrientedSearching(
        IQueryable<(Course course, Subject subject)> courseQuery)
    {
        if (currentUserService.IsAuthenticated && currentUserService.Roles.Contains(WpcConstantValue.TutorRole))
        {
            // TODO: add list of majors into token
            //List<int> tutor = [];

            // TODO: Get taught courses

            // Order by tutor's majors
        }

        return courseQuery;
    }
}