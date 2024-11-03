using MapsterMapper;
using Matt.ResultObject;
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
        var queryable = from tutor in dbContext.Tutors
            where tutor.Id == TutorId.Create(request.TutorId)
            join user in dbContext.Users on tutor.UserId equals user.Id
            join major in dbContext.Majors on tutor.Id equals major.TutorId
            join subject in dbContext.Subjects on major.SubjectId equals subject.Id into majors
            select new TutorDto
            {
                Id = tutor.Id.Value,
                FullName = user.FirstName + " " + user.LastName,
                AcademicLevel = tutor.AcademicLevel.ToString(),
                Address = user.Address.ToString(),
                Avatar = user.Avatar,
                BirthYear = user.BirthYear,
                Description = user.Description,
                Gender = user.Gender.ToString(),
                Rate = tutor.Rate,
                TutorMajors = majors.Select(g => g.Name).ToList()
            };

        var tutorDto = await queryable.FirstOrDefaultAsync(cancellationToken: cancellationToken);

        return tutorDto ?? Result<TutorDto>.Fail(DomainErrors.Tutors.NotFound);
    }
}