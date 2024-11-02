using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Infrastructure.Persistence.EntityFrameworkCore;

namespace WePrepClass.Infrastructure.Persistence.Repositories;

public class ReadDbContext(
    AppDbContext appDbContext
) : IReadDbContext
{
    public DbSet<Tutor> Tutors => appDbContext.Set<Tutor>();
    public DbSet<Major> Majors => appDbContext.Set<Major>();
    public DbSet<User> Users => appDbContext.Set<User>();
    public DbSet<Subject> Subjects => appDbContext.Set<Subject>();
    public DbSet<Course> Courses => appDbContext.Set<Course>();
}