using FluentValidation;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Tutors;
using WePrepClass.Domain;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UseCases.Administrator.Tutors.Commands;

public record UpdateTutorProfileCommand(TutorProfileDto TutorProfileDto) : ICommandRequest;

public class UpdateTutorProfileCommandValidator : AbstractValidator<UpdateTutorProfileCommand>
{
    public UpdateTutorProfileCommandValidator()
    {
        RuleFor(x => x.TutorProfileDto).NotNull();
        RuleFor(x => x.TutorProfileDto).SetValidator(new TutorBasicUpdateDtoValidator());
    }
}

public class UpdateTutorProfileCommandHandler(
    ITutorRepository tutorRepository,
    IUnitOfWork unitOfWork,
    IAppLogger<UpdateTutorProfileCommandHandler> logger
) : CommandHandlerBase<UpdateTutorProfileCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(UpdateTutorProfileCommand command,
        CancellationToken cancellationToken)
    {
        var tutorId = TutorId.Create(command.TutorProfileDto.Id);
        var tutor = await tutorRepository.GetById(tutorId, cancellationToken);

        if (tutor is null) return DomainErrors.Tutors.NotFound;

        tutor.Update(
            command.TutorProfileDto.University,
            command.TutorProfileDto.AcademicLevel.ToEnum<AcademicLevel>(),
            command.TutorProfileDto.MajorIds.Select(SubjectId.Create).ToList()
        );

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}