using FluentValidation;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Contracts.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;

namespace WePrepClass.Application.UseCases.Administrator.Subjects.Commands;

public record UpsertSubjectCommand(SubjectDto SubjectDto) : ICommandRequest;

public class UpsertSubjectCommandValidator : AbstractValidator<UpsertSubjectCommand>
{
    public UpsertSubjectCommandValidator()
    {
        RuleFor(x => x.SubjectDto).NotNull();
        RuleFor(x => x.SubjectDto).SetValidator(new SubjectDtoValidator());
    }
}

public class UpsertSubjectCommandHandler(
    ISubjectRepository subjectRepository,
    IUnitOfWork unitOfWork,
    IAppLogger<UpsertSubjectCommandHandler> logger
) : CommandHandlerBase<UpsertSubjectCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(UpsertSubjectCommand request, CancellationToken cancellationToken)
    {
        var subject =
            await subjectRepository.GetAsync(SubjectId.Create(request.SubjectDto.Id), cancellationToken);

        if (subject is not null)
        {
            var result = SetSubject(request, subject);

            if (result.IsFailure) return result.Error;
        }
        else
        {
            var newSubject = Subject.Create(request.SubjectDto.Name, request.SubjectDto.Description);

            if (newSubject.IsFailure) return newSubject.Error;

            await subjectRepository.InsertAsync(newSubject.Value);
        }

        return await UnitOfWork.SaveChangesAsync(cancellationToken) <= 0
            ? Result.Fail(AppServiceError.Subject.SavingChangesFailed)
            : Result.Success();
    }

    private static Result SetSubject(UpsertSubjectCommand request, Subject subject)
    {
        var setDescriptionResult = subject.SetDescription(request.SubjectDto.Description);
        var setNameResult = subject.SetName(request.SubjectDto.Name);
        
        if (setDescriptionResult.IsFailure) return setDescriptionResult.Error;
        if (setNameResult.IsFailure) return setNameResult.Error;
        
        return Result.Success();
    }
}