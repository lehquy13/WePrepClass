using MapsterMapper;
using Matt.ResultObject;
using Matt.SharedKernel.Application.Mediators;
using Matt.SharedKernel.Application.Mediators.Queries;
using Matt.SharedKernel.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Contracts.Tutors;
using WePrepClass.Domain;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Application.UseCases.Wpc.Tutors.Queries;

public record GetTutorQuery(Guid TutorId) : IQueryRequest<TutorDto>;

public class GetTutorQueryHandler(
    IReadDbContext dbContext,
    IAppLogger<GetTutorQueryHandler> logger,
    IMapper mapper
) : QueryHandlerBase<GetTutorQuery, TutorDto>(logger, mapper)
{
    public override async Task<Result<TutorDto>> Handle(GetTutorQuery request, CancellationToken cancellationToken)
    {
        var tutorDto = await dbContext.Tutors
            .Where(t => t.Id == TutorId.Create(request.TutorId))
            .Join(dbContext.Users,
                tutor => tutor.UserId,
                user => user.Id,
                (tutor, user) => new
                {
                    tutor,
                    user
                }
            )
            .Join(dbContext.Majors,
                tutorUser => tutorUser.tutor.Id,
                major => major.TutorId,
                (tutorUser, major) => new
                {
                    tutorUser.tutor,
                    tutorUser.user,
                    major
                }
            )
            .GroupJoin(dbContext.Subjects,
                tutorUserMajor => tutorUserMajor.major.SubjectId,
                subject => subject.Id,
                (tutorUserMajor, subject) => new
                {
                    tutorUserMajor.tutor,
                    tutorUserMajor.user,
                    subjects = subject
                }
            )
            .Select(group => new TutorDto
            {
                Id = group.tutor.Id.Value,
                FullName = group.user.FirstName + " " + group.user.LastName,
                AcademicLevel = group.tutor.AcademicLevel.ToString(),
                Address = group.user.Address.ToString(),
                Avatar = group.user.Avatar,
                BirthYear = group.user.BirthYear,
                Description = group.user.Description,
                Gender = group.user.Gender.ToString(),
                Rate = group.tutor.Rate,
                TutorMajors = group.subjects.Select(s => s.Name).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return tutorDto ?? Result<TutorDto>.Fail(DomainErrors.Tutors.NotFound);
    }
}