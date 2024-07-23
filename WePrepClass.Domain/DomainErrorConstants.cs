using Matt.ResultObject;

namespace WePrepClass.Domain;

public static class DomainErrorConstants
{
    public static class Tutor
    {
        public const string InvalidImageUrl = "Invalid image url";

        public static readonly Error VerificationChangeCantBeEmpty =
            new("VerificationChangesCantBeEmpty", "At least one verification change is required");
    }
}