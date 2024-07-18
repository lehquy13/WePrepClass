using Matt.SharedKernel.Application.Contracts.Primitives;

namespace WePrepClass.Contracts;

public abstract class BasicAuditedEntityDto<TId> : EntityDto<TId>, IAuditDto
    where TId : notnull
{
    public DateTime? LastModificationTime { get; set; } = DateTime.Now;
    public DateTime CreationTime { get; set; } = DateTime.Now;
}

public interface IAuditDto
{
    public DateTime? LastModificationTime { get; set; }
    public DateTime CreationTime { get; set; }
}