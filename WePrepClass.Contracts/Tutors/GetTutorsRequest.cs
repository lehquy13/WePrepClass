// ReSharper disable UnassignedGetOnlyAutoProperty

using Matt.SharedKernel.Paginations;

namespace WePrepClass.Contracts.Tutors;

public class GetTutorsRequest : PaginatedParams
{
    public string? Subject { get; }
    public int? BirthYear { get; }
    public string? Academic { get; }
    public string? Gender { get; }
    public string? City { get; }
    public string? District { get; }
}