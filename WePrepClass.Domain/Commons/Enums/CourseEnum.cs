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
    Optional,
    UnderGraduate,
    Graduated,
    Lecturer
}

public enum TutorStatus
{
    Active,
    InActive
}

public static class CurrencyCode
{
    public static string USD = "USD";
    public static string VND = "VND";
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