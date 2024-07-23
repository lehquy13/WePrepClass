using Matt.SharedKernel.Domain.Primitives.Auditing;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;

public class Verification : AuditedEntity<VerificationId>
{
    public TutorId TutorId { get; private set; } = null!;
    public string Image { get; private set; } = null!;
    
    private Verification()
    {
    }

    public static Verification Create(string image, TutorId tutorId)
    {
        return new Verification()
        {
            Id = VerificationId.Create(),
            TutorId = tutorId,
            Image = image
        };
    }
}