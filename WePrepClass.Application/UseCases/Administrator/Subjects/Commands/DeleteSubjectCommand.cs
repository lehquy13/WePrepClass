using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Commands;
using Matt.SharedKernel.Domain.Interfaces;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Subjects.ValueObjects;

namespace WePrepClass.Application.UseCases.Administrator.Subjects.Commands;

public record DeleteSubjectCommand(int SubjectId) : ICommandRequest;

public class DeleteSubjectCommandHandler(
    ISubjectRepository subjectRepository,
    IUnitOfWork unitOfWork,
    IAppLogger<DeleteSubjectCommandHandler> logger
) : CommandHandlerBase<DeleteSubjectCommand>(unitOfWork, logger)
{
    public override async Task<Result> Handle(DeleteSubjectCommand request, CancellationToken cancellationToken)
    {
        var subjectExists = await subjectRepository.GetAsync(SubjectId.Create(request.SubjectId), cancellationToken);

        if (subjectExists is null) return Result.Fail(AppServiceError.Subject.NotExist);

        subjectExists.SetAsDeleted();

        return await UnitOfWork.SaveChangesAsync(cancellationToken) <= 0
            ? Result.Fail(AppServiceError.Subject.SavingChangesFailed)
            : Result.Success();
    }
}