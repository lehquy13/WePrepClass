using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Profiles.Queries;

public record GetLearningCoursesQuery : IQueryRequest<IEnumerable<AttendedCourseListDto>>, IAuthorizationRequired;

public class GetLearningCoursesQueryHandler(
    IReadDbContext dbContext,
    ICurrentUserService currentUserService
) : QueryHandlerBase<GetLearningCoursesQuery, IEnumerable<AttendedCourseListDto>>
{
    public override async Task<Result<IEnumerable<AttendedCourseListDto>>> Handle(GetLearningCoursesQuery request,
        CancellationToken cancellationToken)
    {
        var query = from course in dbContext.Courses
            join subject in dbContext.Subjects on course.SubjectId equals subject.Id
            where course.LearnerDetail.LearnerId == UserId.Create(currentUserService.UserId)
            select new AttendedCourseListDto
            {
                Id = course.Id.Value,
                Title = course.Title,
                SubjectName = subject.Name,
                Status = course.Status.ToString()
            };

        return await query.ToListAsync(cancellationToken);
    }
}