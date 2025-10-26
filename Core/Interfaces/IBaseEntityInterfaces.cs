namespace Core.Interfaces;


public interface IBaseEntity<T>
{
    public T Id { get; set; }
}
public interface ICreateDate
{
    public DateTime CreatedDate { get; set; }
}
public interface ISoftDelete
{
    public bool IsDeleted { get; set; }
}
public interface IUpdateDate
{
    public DateTime LastUpdateDate { get; set; }
}
public interface IBaseEntityWithDate<T> : ICreateDate, IBaseEntity<T>;