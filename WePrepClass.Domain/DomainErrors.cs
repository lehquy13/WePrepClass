using Matt.ResultObject;

namespace WePrepClass.Domain;

public static class DomainErrors
{
    public static class Tutors
    {
        public static readonly Error InvalidImageUrl = new("InvalidImageUrl", "Invalid image url format");

        public static readonly Error VerificationChangeCantBeEmpty =
            new("VerificationChangesCantBeEmpty", "At least one verification change is required");
    }

    public static class Users
    {
        public static readonly Error InvalidPhoneNumber = new("InvalidPhoneNumber", "Invalid phone number format");
        public static readonly Error InvalidBirthYear = new("InvalidBirthYear", "Birth year is invalid");
        public static readonly Error FirstNameIsRequired = new("FirstNameIsRequired", "First name is required");
        public static readonly Error LastNameIsRequired = new("LastNameIsRequired", "Last name is required");
        public static readonly Error DescriptionIsRequired = new("DescriptionIsRequired", "Description is required");
        public static readonly Error EmailIsRequired = new("EmailIsRequired", "Email is required");
    }

    public static class Courses
    {
        public static readonly Error InvalidDetailLength =
            new("InvalidDetailLength", "Detail length should be less than 500");

        public static readonly Error CourseUnavailable = new("CourseUnavailable", "Course is not available");
        public static readonly Error InvalidReviewRate = new("InvalidReviewRate", "Rate should be between 1 and 5");

        public static readonly Error SessionDurationOutOfRange =
            new("SessionDurationOutOfRange", "Session duration should be at least 60 minutes");

        public static readonly Error SessionPerWeekOutOfRange =
            new("SessionPerWeekOutOfRange", "Session per week should be between 1 and 7");

        public static readonly Error CourseTitleTooShort =
            new("TitleTooShort", "Title should be at least 50 characters");

        public static readonly Error CourseNotBeenConfirmed =
            new("CourseNotBeenConfirmed", "Course has not been confirmed");

        public static readonly Error TeachingRequestAlreadyExist =
            new("TeachingRequestAlreadyExist", "Teaching request already exists");

        public static readonly Error TutorAndLearnerShouldNotBeTheSame =
            new("TutorAndLearnerShouldNotBeTheSame", "Tutor and learner should not be the same");

        public static readonly Error CourseStatusInvalidForUnassignment = new("CourseStatusInvalidForUnassignment",
            "Course can not unassign tutor due to having been cancelled or completed");

        public static readonly Error CourseNotBeenAssigned = new("UnavailableForConfirmation",
            "Course is not available for confirmation");

        public static readonly Error ReviewNotAllowedYet = new("CourseReviewNotAllowedYet",
            "Course review is not allowed yet");
    }
}