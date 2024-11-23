using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Courses;

namespace WePrepClass.Application.UseCases.Administrator.Courses.Queries;

public record GetTodayCoursesQuery : IQueryRequest<IEnumerable<CourseListDto>>;

public class GetTodayCoursesQueryHandler(
    IReadDbContext dbContext,
    IAppLogger<GetTodayCoursesQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetTodayCoursesQuery, IEnumerable<CourseListDto>>(logger, mapper)
{
    public override async Task<Result<IEnumerable<CourseListDto>>> Handle(GetTodayCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var courseQuery =
            from course in dbContext.Courses.Where(x =>
                x.IsDeleted == false && x.CreationTime >= DateTime.Today.AddDays(-1))
            join subject in dbContext.Subjects on course.SubjectId equals subject.Id
            select new CourseListDto
            {
                Id = course.Id.Value,
                Title = course.Title,
                Status = course.Status.ToString(),
                CreationTime = course.CreationTime,
                LearningMode = course.LearningModeRequirement.ToString(),
                SubjectName = subject.Name
            };

        return await courseQuery.ToListAsync(cancellationToken);
    }
}