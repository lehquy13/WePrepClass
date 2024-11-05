using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Courses;
using WePrepClass.Domain;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Courses.Queries;

public record GetCourseQuery(Guid CourseId) : IQueryRequest<CourseDto>;

public class GetCourseDetailQueryHandler(
    IReadDbContext dbContext,
    IAppLogger<GetCourseDetailQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetCourseQuery, CourseDto>(logger, mapper)
{
    public override async Task<Result<CourseDto>> Handle(GetCourseQuery request,
        CancellationToken cancellationToken)
    {
        var queryable =
            from course in dbContext.Courses
            join subject in dbContext.Subjects on course.SubjectId equals subject.Id
            where course.Id == CourseId.Create(request.CourseId)
            select new CourseDto
            {
                Title = course.Title,
                Description = course.Description,
                Status = course.Status.ToString(),
                LearningMode = course.LearningModeRequirement.ToString(),
                SectionFee = course.SessionFee.Display,
                ChargeFee = course.ChargeFee.Display,
                GenderRequirement = course.TutorSpecification.TutorGender.ToString(),
                AcademicLevelRequirement = course.TutorSpecification.TutorAcademicLevel.ToString(),
                LearnerGender = course.LearnerDetail.LearnerGender.ToString()
            };

        var courseFromDb = await queryable.FirstOrDefaultAsync(cancellationToken);

        if (courseFromDb is null)
        {
            return Result.NotFound(DomainErrors.Courses.NotFound.Code, DomainErrors.Courses.NotFound.Description);
        }

        return courseFromDb;
    }
}