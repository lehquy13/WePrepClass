using Matt.SharedKernel.Domain.Primitives;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Domain.WePrepClassAggregates.Courses.ValueObjects;

public class LearnerDetail : ValueObject
{
    public Gender LearnerGender { get; private set; } = Gender.Male;
    public string LearnerName { get; private set; } = string.Empty;
    public int NumberOfLearner { get; private set; } = 1;
    public string ContactNumber { get; private set; } = string.Empty;
    public UserId? LearnerId { get; private set; }

    private LearnerDetail()
    {
    }

    public static LearnerDetail Create(string learnerName,
        Gender learnerGender,
        string contactNumber,
        int numberOfLearner = 1,
        UserId? learnerId = null)
    {
        return new LearnerDetail
        {
            LearnerGender = learnerGender,
            LearnerName = learnerName,
            NumberOfLearner = numberOfLearner,
            ContactNumber = contactNumber,
            LearnerId = learnerId
        };
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return LearnerGender;
        yield return LearnerName;
        yield return NumberOfLearner;
        yield return ContactNumber;
        yield return LearnerId!;
    }
}