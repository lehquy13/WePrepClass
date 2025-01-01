using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain;
using Matt.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Tutors;
using WePrepClass.Domain.Commons.Enums;

namespace WePrepClass.Application.UseCases.Wpc.Courses.Queries;

public record GetPopularTutorsQuery : IQueryRequest<IEnumerable<TutorListDto>>;

public class GetPopularTutorsQueryHandler(
    IReadDbContext dbContext
) : QueryHandlerBase<GetPopularTutorsQuery, IEnumerable<TutorListDto>>
{
    public override async Task<Result<IEnumerable<TutorListDto>>> Handle(GetPopularTutorsQuery request,
        CancellationToken cancellationToken)
    {
        var tutorIds =
            from assignment in dbContext.TeachingAssignments
            where assignment.TeachingAssignmentStatus == TeachingAssignmentStatus.Assigned &&
                  assignment.CreationTime > DateTimeProvider.Now.AddMonths(-1)
            group assignment by assignment.TutorId
            into tutorGroup
            orderby tutorGroup.Count() descending
            select tutorGroup.Key;

        var queryable =
            from tutor in dbContext.Tutors
            where tutorIds.Contains(tutor.Id)
            join user in dbContext.Users on tutor.UserId equals user.Id
            select new TutorListDto
            {
                Id = tutor.Id.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Avatar = user.Avatar,
                Description = user.Description,
                Rate = tutor.Rate
            };

        return await queryable.ToListAsync(cancellationToken);
    }
}