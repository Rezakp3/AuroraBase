namespace Core.Entities;

public interface IBaseEntity<T>
{
    public T Id { get; set; }
}
public interface IBaseEntityWithDate<T> : IBaseEntity<T>
{
    public DateTime CreatedDate { get; set; }
}

public class BaseEntity<T> : IBaseEntity<T>
{
    public T Id { get; set; }
}

public class BaseEntityWithDate<T> : BaseEntity<T>, IBaseEntityWithDate<T>
{
    public DateTime CreatedDate { get; set; }
}
