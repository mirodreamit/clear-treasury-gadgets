namespace CT.Domain.Abstractions.Interfaces
{
    public interface IAuditableEntityWithSoftDelete
    {
        public bool IsDeleted { get; set; }
    }
}
