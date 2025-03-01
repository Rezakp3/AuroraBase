using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;
public interface IEntity<TKey>
{
    public TKey Id { get; set; }
}
public class BaseEntity<TKey> : IEntity<TKey>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public required TKey Id { get; set; }
}
