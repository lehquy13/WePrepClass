namespace WePrepClass.Domain.Commons.Enums;

public enum CourseStatus
{
    None = 0,
    Refunded = 1,
    Cancelled = 2,
    Confirmed = 3,
    PendingApproval = 4,
    Available = 5,
    InProgress = 6
}

public enum AcademicLevel
{
    UnderGraduate,
    Graduated,
    Lecturer
}

public enum AcademicLevelOption
{
    Optional,
    UnderGraduate,
    Graduated,
    Lecturer
}

public enum TutorStatus
{
    Active,
    Inactive,
    Unproven
}

public static class CurrencyCode
{
    public const string Usd = "USD";
    public const string Vnd = "VND";
}

public enum LearningMode
{
    Online,
    Offline,
    Hybrid
}

public enum DurationUnit
{
    Minute,
    Hour
}

public enum SessionFrequency
{
    Daily,
    Weekly,
    Monthly,
    Custom
}

public enum RequestStatus
{
    InProgress,
    Approved,
    Denied
}