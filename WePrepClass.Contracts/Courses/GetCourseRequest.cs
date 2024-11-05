using Matt.Paginated;

namespace WePrepClass.Contracts.Courses;

public sealed class GetCourseRequest : PaginatedParams
{
    public string? SubjectName { get; set; }
}