using ESCenter.Client.Application.ServiceImpls.TutorProfiles;
using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Contracts.Interfaces;
using Matt.SharedKernel.Application.Contracts.Interfaces.Infrastructures;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Domain;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.TutorProfiles.Queries;

public record GetTutorProfileQuery() : IQueryRequest<TutorForProfileDto>, IAuthorizationRequired;

public class GetTutorProfileQueryHandler(
    IReadDbContext dbContext,
    ICurrentUserService currentUserService,
    IMapper mapper,
    IAppLogger<GetTutorProfileQueryHandler> logger)
    : QueryHandlerBase<GetTutorProfileQuery, TutorForProfileDto>(logger, mapper)
{
    public override async Task<Result<TutorForProfileDto>> Handle(GetTutorProfileQuery request,
        CancellationToken cancellationToken)
    {
       var tutorQueryable =
    from tutor in dbContext.Tutors
    join user in dbContext.Users on tutor.UserId equals user.Id
    join major in dbContext.Majors on tutor.Id equals major.TutorId
    join subject in dbContext.Subjects on major.SubjectId equals subject.Id 
    join verification in dbContext.Verifications on tutor.Id equals verification.TutorId into verifications
    join verificationChange in dbContext.VerificationChanges on tutor.Id equals verificationChange.TutorId into verificationChanges
    where tutor.Id == TutorId.Create(currentUserService.UserId)
    select new TutorForProfileDto
    {
        FirstName = user.FirstName,
        LastName = user.LastName,
        Description = user.Description,
        University = tutor.University,
        AcademicLevel = tutor.AcademicLevel.ToString(),
        IsVerified = tutor.TutorStatus == TutorStatus.Active,
        Rate = tutor.Rate,
        VerificationDtos = verifications
            .Select(v => new VerificationDto
            {
                Id = v.Id.Value,
                Image = v.Image,
                CreationTime = v.CreationTime,
                LastModificationTime = v.LastModificationTime
            })
            .ToList(),
        Majors = dbContext.Majors
            .Where(m => m.TutorId == tutor.Id)
            .Select(m => new TutorMajorDto
            {
                IsMajored = true,
                SubjectId = m.SubjectId.Value,
                SubjectName = dbContext.Subjects.FirstOrDefault(s => s.Id == m.SubjectId).Name
            })
            .ToList(),
        ChangeVerificationRequestDtos = verificationChanges
            .Select(x => new ChangeVerificationRequestDto
            {
                Id = x.Id.Value,
                RequestStatus = x.VerificationChangeStatus.ToString(),
                ChangeVerificationRequestDetails = x.ChangeVerificationRequestDetails
                    .Select(y => y.ImageUrl)
                    .ToList()
            })
            .ToList()
    };

        var tutorResult = await tutorQueryable.FirstOrDefaultAsync(cancellationToken);

        if (tutorResult is null) return Result.Fail(DomainErrors.Tutors.NotFound);

        foreach (var subject in dbContext.Subjects.ToList())
        {
            if (tutorResult.Majors.Any(x => x.SubjectId == subject.Id.Value))
            {
                tutorResult.Majors.First(x => x.SubjectId == subject.Id.Value).SubjectName = subject.Name;
            }

            tutorResult.Majors.Add(new TutorMajorDto
            {
                IsMajored = false,
                SubjectId = subject.Id.Value,
                SubjectName = subject.Name
            });
        }

        return tutorResult;
    }
}