using System.Linq.Expressions;
using Application.Common.Interfaces.Generals;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class Repository<TEntity, TKey> : IRepository<TEntity, TKey> 
    where TEntity : class
{
    protected readonly MyContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public Repository(MyContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync([id], cancellationToken);

    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    public virtual async Task<List<TEntity>> GetWhereAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate, 
        CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(predicate, cancellationToken);

    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null, 
        CancellationToken cancellationToken = default)
        => predicate == null 
            ? await _dbSet.CountAsync(cancellationToken) 
            : await _dbSet.CountAsync(predicate, cancellationToken);

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await _dbSet.AddRangeAsync(entities, cancellationToken);

    public virtual void Update(TEntity entity)
        => _dbSet.Update(entity);

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
        => _dbSet.UpdateRange(entities);

    public virtual void Delete(TEntity entity)
        => _dbSet.Remove(entity);

    public virtual void DeleteRange(IEnumerable<TEntity> entities)
        => _dbSet.RemoveRange(entities);

    public virtual async Task<bool> DeleteByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return false;

        Delete(entity);
        return true;
    }
}