
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using Matt.SharedKernel.Results;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Tutors;
using WePrepClass.Domain;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.TutorProfiles.Commands;

public record UpdateTutorInformationCommand(TutorBasicUpdateForClientDto TutorBasicUpdateDto) : ICommandRequest;

public class UpdateTutorInformationCommandHandler(
    IReadDbContext dbContext,
    ITutorRepository tutorRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService
) : CommandHandlerBase<UpdateTutorInformationCommand>(unitOfWork)
{
    public override async Task<Result> Handle(UpdateTutorInformationCommand command,
        CancellationToken cancellationToken)
    {
        var tutor = await tutorRepository.GetById(TutorId.Create(currentUserService.UserId), cancellationToken);

        if (tutor is null) return Result.Fail(DomainErrors.Tutors.NotFound);

        var subjectsToUpdate = await dbContext.Subjects
            .Where(x => command.TutorBasicUpdateDto.MajorIds.Contains(x.Id.Value))
            .ToListAsync(cancellationToken);

        tutor.Update(
            command.TutorBasicUpdateDto.University,
            command.TutorBasicUpdateDto.AcademicLevel.ToEnum<AcademicLevel>(),
            subjectsToUpdate.Select(x => x.Id).ToList()
        );

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}