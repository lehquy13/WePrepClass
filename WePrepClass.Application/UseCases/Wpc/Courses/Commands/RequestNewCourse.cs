using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Courses;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Courses.Commands;

public record RequestNewCourseCommand(CourseCreateDto CourseToCreate) : ICommandRequest;

public class RequestNewCourseCommandHandler(
    ICourseRepository courseRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IAppLogger<RequestNewCourseCommandHandler> logger
) : CommandHandlerBase<RequestNewCourseCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(RequestNewCourseCommand command,
        CancellationToken cancellationToken)
    {
        var session = Session.Create(
            command.CourseToCreate.SessionPerWeek,
            command.CourseToCreate.SessionDuration);

        var address = Address.Create(
            command.CourseToCreate.City,
            command.CourseToCreate.Country,
            command.CourseToCreate.Address
        );

        if (session.IsFailed || address.IsFailed)
        {
            return Result.Fail("Invalid session or address");
        }

        var course = Course.Create(
            command.CourseToCreate.Title,
            command.CourseToCreate.Description,
            command.CourseToCreate.LearningModeRequirement,
            Fee.Create(command.CourseToCreate.Fee, CurrencyCode.Usd),
            Fee.Create(command.CourseToCreate.Fee, CurrencyCode.Usd),
            LearnerDetail.Create(
                command.CourseToCreate.LearnerName,
                command.CourseToCreate.LearnerGender,
                command.CourseToCreate.ContactNumber,
                command.CourseToCreate.NumberOfLearner,
                UserId.Create(currentUserService.UserId)),
            TutorSpecification.Create(
                command.CourseToCreate.GenderRequirement,
                command.CourseToCreate.AcademicLevelRequirement
            ),
            session.Value,
            address.Value,
            SubjectId.Create(command.CourseToCreate.SubjectId));

        if (course.IsFailed) return course.Error;

        courseRepository.Insert(course.Value);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}