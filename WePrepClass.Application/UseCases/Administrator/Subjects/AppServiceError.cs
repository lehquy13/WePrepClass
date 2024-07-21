using Matt.ResultObject;

namespace WePrepClass.Application.UseCases.Administrator.Subjects;

public static class AppServiceError
{
    public static class Subject
    {
        public static Error SavingChangesFailed => new("SavingChangesFailed", "Failed to save changes");
        public static Error NotExist => new("NonExistSubjectError", "Non-exist subject");
    }
}