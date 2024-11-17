using FluentValidation;
using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Mediators;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Users;
using WePrepClass.Domain;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Profiles.Queries;

public record GetLearningCourseDetailQuery(Guid CourseId)
    : IQueryRequest<AttendedCourseDetailDto>, IAuthorizationRequired;

public class GetLearningCourseDetailQueryValidator : AbstractValidator<GetLearningCourseDetailQuery>
{
    public GetLearningCourseDetailQueryValidator()
    {
        RuleFor(x => x.CourseId).NotEmpty();
    }
}

public class GetLearningCourseDetailQueryHandler(
    IReadDbContext dbContext,
    IAppLogger<RequestHandlerBase> logger,
    IMapper mapper
) : QueryHandlerBase<GetLearningCourseDetailQuery, AttendedCourseDetailDto>(logger, mapper)
{
    public override async Task<Result<AttendedCourseDetailDto>> Handle(GetLearningCourseDetailQuery request,
        CancellationToken cancellationToken)
    {
        var courseId = CourseId.Create(request.CourseId);

        var courseRequestQueryable =
            from course in dbContext.Courses
            join teachingAssignment in dbContext.TeachingAssignments on course.Id equals teachingAssignment.CourseId
            join tutor in dbContext.Tutors on teachingAssignment.TutorId equals tutor.Id
            join user in dbContext.Users on tutor.UserId equals user.Id
            join subject in dbContext.Subjects on course.SubjectId equals subject.Id
            where course.Id == courseId
            select new AttendedCourseDetailDto
            {
                Id = course.Id.Value,
                Title = course.Title,
                Description = course.Description,
                SubjectName = subject.Name,
                Status = course.Status.ToString(),
                LearningMode = course.LearningModeRequirement.ToString(),
                ChargeFee = course.ChargeFee.Amount,
                SectionFee = course.SessionFee.Display,
                Address = course.Address.ToString(),
                Rate = course.Review == null ? (short)5 : course.Review.Rate,
                Detail = course.Review == null ? "" : course.Review.Detail,
                TutorId = user.Id.Value,
                TutorName = user.GetFullName(),
                TutorContact = user.PhoneNumber,
                TutorEmail = user.Email
            };

        var courseQueryResult =
            await courseRequestQueryable.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (courseQueryResult is null)
        {
            return DomainErrors.Courses.NotFound;
        }

        return courseQueryResult;
    }
}