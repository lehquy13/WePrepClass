using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WePrepClass.Domain.WePrepClassAggregates.Notifications;

namespace WePrepClass.Infrastructure.Persistence.EntityFrameworkCore.Configs;

internal class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable(nameof(Notification));
        builder.HasKey(r => r.Id);
        builder.Property(r => r.ObjectId).IsRequired();
        builder.Property(r => r.IsRead).IsRequired();
        builder.Property(r => r.Message).IsRequired();
    }
}