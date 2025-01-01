using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Courses.Queries;

public record GetRelatedCoursesQuery(
    Guid CourseId,
    int SubjectId
) : IQueryRequest<IEnumerable<CourseListDto>>;

public class GetRelatedCoursesQueryHandler(
    IReadDbContext readDbContext
) : QueryHandlerBase<GetRelatedCoursesQuery, IEnumerable<CourseListDto>>
{
    public override async Task<Result<IEnumerable<CourseListDto>>> Handle(GetRelatedCoursesQuery request,
        CancellationToken cancellationToken)
    {
        return await (
                from course in readDbContext.Courses
                join subject in readDbContext.Subjects on course.SubjectId equals subject.Id
                where course.SubjectId == SubjectId.Create(request.SubjectId) &&
                      course.Id != CourseId.Create(request.CourseId)
                select new CourseListDto
                {
                    Id = course.Id.Value,
                    Title = course.Title,
                    Status = course.Status.ToString(),
                    LearningMode = course.LearningModeRequirement.ToString(),
                    SubjectName = subject.Name
                })
            .Take(5)
            .ToListAsync(cancellationToken);
    }
}