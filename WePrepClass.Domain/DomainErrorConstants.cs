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

    public static class User
    {
        public static readonly Error InvalidPhoneNumber = new("InvalidPhoneNumber", "Invalid phone number format");

        public static readonly Error InvalidBirthYear = new("InvalidBirthYear", "Birth year is invalid");

        public static readonly Error FirstNameIsRequired = new("FirstNameIsRequired", "First name is required");

        public static readonly Error LastNameIsRequired = new("LastNameIsRequired", "Last name is required");

        public static readonly Error DescriptionIsRequired = new("DescriptionIsRequired", "Description is required");

        public static readonly Error EmailIsRequired = new("EmailIsRequired", "Email is required");
    }
}