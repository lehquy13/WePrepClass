using Microsoft.EntityFrameworkCore;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Users;

namespace WePrepClass.Application.Interfaces;

public interface IReadDbContext
{
    DbSet<Tutor> Tutors { get; }
    DbSet<Major> Majors { get; }
    DbSet<User> Users { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<Course> Courses { get; }
}