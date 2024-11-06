using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Tutors;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.DomainServices;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Profiles.Commands;

public record EnrollAsTutorCommand(TutorEnrollmentDto TutorRegistrationDto) : ICommandRequest;

public class EnrollAsTutorCommandHandler(
    IUnitOfWork unitOfWork,
    ITutorDomainService tutorDomainService,
    ICurrentUserService currentUserService,
    IAppLogger<EnrollAsTutorCommandHandler> logger
) : CommandHandlerBase<EnrollAsTutorCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(EnrollAsTutorCommand command, CancellationToken cancellationToken)
    {
        var result = await tutorDomainService.EnrollAsTutor(
            UserId.Create(currentUserService.UserId),
            command.TutorRegistrationDto.AcademicLevel.ToEnum<AcademicLevel>(),
            command.TutorRegistrationDto.University,
            command.TutorRegistrationDto.MajorIds.Select(SubjectId.Create).ToList(),
            cancellationToken);

        if (result.IsFailed) return Result.Fail(result.Error);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}