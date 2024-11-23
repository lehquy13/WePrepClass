using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Subjects;
using WePrepClass.Contracts.Tutors;
using WePrepClass.Domain;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UseCases.Administrator.Tutors.Queries;

public record GetTutorQuery(Guid TutorId) : IQueryRequest<TutorDetailDto>;

public class GetTutorQueryHandler(
    IReadDbContext dbContext,
    IAppLogger<GetTutorQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetTutorQuery, TutorDetailDto>(logger, mapper)
{
    public override async Task<Result<TutorDetailDto>> Handle(GetTutorQuery request,
        CancellationToken cancellationToken)
    {
        var queryable = from tutor in dbContext.Tutors
            where tutor.Id == TutorId.Create(request.TutorId)
            join user in dbContext.Users on tutor.UserId equals user.Id
            join major in dbContext.Majors on tutor.Id equals major.TutorId
            join subject in dbContext.Subjects on major.SubjectId equals subject.Id into majors
            select new TutorDetailDto
            {
                Id = tutor.Id.Value,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AcademicLevel = tutor.AcademicLevel.ToString(),
                Avatar = user.Avatar,
                BirthYear = user.BirthYear,
                Description = user.Description,
                Gender = user.Gender.ToString(),
                University = tutor.University,
                IsVerified = tutor.TutorStatus == TutorStatus.Active,
                Majors = majors.Select(x => new MajorDto
                {
                    Name = x.Name,
                    IsSelected = true,
                }).ToList(),
                Verifications = tutor.Verifications.Select(x => new VerificationDto
                {
                    Image = x.Image
                }).ToList(),
                VerificationChanges = tutor.VerificationChanges.Select(x => new ChangeVerificationRequestDto
                {
                    Id = x.Id.Value,
                    RequestStatus = x.VerificationChangeStatus.ToString(),
                    ChangeVerificationRequestDetails =
                        x.ChangeVerificationRequestDetails.Select(y => y.ImageUrl).ToList()
                }).ToList()
            };

        var tutorDto = await queryable.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (tutorDto == null) return Result.Fail(DomainErrors.Tutors.NotFound);

        return tutorDto;
    }
}