using Core.Interfaces;

namespace Core.Entities;

public class BaseEntity<T> : IBaseEntity<T>
{
    public T Id { get; set; }
}

public class BaseEntityWithDate<T> : BaseEntity<T>, IBaseEntityWithDate<T>
{
    public DateTime CreatedDate { get; set; }
}
