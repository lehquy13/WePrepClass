using Microsoft.EntityFrameworkCore;
using WePrepClass.Application.Interfaces;
using WePrepClass.Domain.WePrepClassAggregates.Courses;
using WePrepClass.Domain.WePrepClassAggregates.Courses.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Subjects;
using WePrepClass.Domain.WePrepClassAggregates.TutoringRequests;
using WePrepClass.Domain.WePrepClassAggregates.Tutors;
using WePrepClass.Domain.WePrepClassAggregates.Tutors.Entities;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Infrastructure.Persistence.EntityFrameworkCore;

namespace WePrepClass.Infrastructure.Persistence.Repositories;

public class ReadDbContext(
    AppDbContext appDbContext
) : IReadDbContext
{
    public DbSet<Tutor> Tutors => appDbContext.Set<Tutor>().AsReadOnly();
    public DbSet<Major> Majors => appDbContext.Set<Major>().AsReadOnly();
    public DbSet<User> Users => appDbContext.Set<User>().AsReadOnly();
    public DbSet<Subject> Subjects => appDbContext.Set<Subject>().AsReadOnly();
    public DbSet<Course> Courses => appDbContext.Set<Course>().AsReadOnly();
    public DbSet<TutoringRequest> TutoringRequests => appDbContext.Set<TutoringRequest>().AsReadOnly();
    public DbSet<Verification> Verifications => appDbContext.Set<Verification>().AsReadOnly();
    public DbSet<VerificationChange> VerificationChanges => appDbContext.Set<VerificationChange>().AsReadOnly();
    public DbSet<TeachingRequest> TeachingRequests => appDbContext.Set<TeachingRequest>().AsReadOnly();
    public DbSet<TeachingAssignment> TeachingAssignments => appDbContext.Set<TeachingAssignment>().AsReadOnly();
}

public static class ReadDbContextExtensions
{
    public static DbSet<T> AsReadOnly<T>(this DbSet<T> dbSet) where T : class => (DbSet<T>)dbSet.AsNoTracking();
}