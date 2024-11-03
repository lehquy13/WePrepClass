using Matt.ResultObject;

namespace WePrepClass.Application.UseCases.Administrator.Subjects;

public static class AppServiceError
{
    public static class Subject
    {
        public static Error SavingChangesFailed => new("SavingChangesFailed", "Failed to save changes");
        public static Error NotFound => new("SubjectNotFound", "Subject is not found");
    }

    public static class Tutors
    {
        public static Error NotFound => new("TutorNotFound", "Tutor is not found");
    }
}