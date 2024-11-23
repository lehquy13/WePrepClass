using ESCenter.Admin.Application.ServiceImpls.Tutors;
using FluentValidation;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Tutors;
using WePrepClass.Contracts.Users;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Administrator.Tutors.Commands;

public record CreateTutorCommand(
    UserProfileDto UserProfileDto,
    TutorProfileDto TutorProfileDto
) : ICommandRequest;

public class CreateTutorCommandValidator : AbstractValidator<CreateTutorCommand>
{
    public CreateTutorCommandValidator()
    {
        RuleFor(x => x.TutorProfileDto).SetValidator(new TutorBasicUpdateDtoValidator());
    }
}

public class CreateTutorCommandHandler(
    ITutorRepository tutorRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IAppLogger<RequestHandlerBase> logger,
    IIdentityService identityService
) : CommandHandlerBase<CreateTutorCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(CreateTutorCommand command, CancellationToken cancellationToken)
    {
        var address = Address.Create(
            command.UserProfileDto.City,
            command.UserProfileDto.Country,
            command.UserProfileDto.DetailAddress);

        if (address.IsFailed) return Result.Fail(address.Error);

        var userInformation = await identityService.CreateAsync(
            command.UserProfileDto.Email,
            command.UserProfileDto.FirstName,
            command.UserProfileDto.LastName,
            command.UserProfileDto.Gender.ToEnum<Gender>(),
            command.UserProfileDto.BirthYear,
            address.Value,
            command.UserProfileDto.Description,
            string.Empty,
            command.UserProfileDto.Email,
            command.UserProfileDto.PhoneNumber,
            Role.Tutor);

        if (userInformation.IsFailed) return userInformation.Error;

        await userRepository.InsertAsync(userInformation.Value, cancellationToken);

        var tutor = Tutor.Create(
            userInformation.Value.Id,
            command.TutorProfileDto.AcademicLevel.ToEnum<AcademicLevel>(),
            command.TutorProfileDto.University,
            command.TutorProfileDto.MajorIds.Select(SubjectId.Create).ToList(),
            TutorStatus.Active);

        if (tutor.IsFailed) return Result.Fail(tutor.Error);

        tutorRepository.Insert(tutor.Value);

        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}