using System.Linq.Expressions;

using Core.Entities;
using Core.Repositories;

using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected DbContextProject _context;
    protected DbSet<TEntity> _dbSet;

    public BaseRepository(DbContextProject context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
        => await _dbSet.AsNoTracking().ToListAsync();

    public async Task<IEnumerable<TEntity>> SearchByConditionAsync(Expression<Func<TEntity, bool>> expression)
        => await _dbSet.AsNoTracking().Where(expression).ToListAsync();

    public async Task<TEntity?> GetByIdAsync(int? id)
        => await _dbSet.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);

    public void Add(TEntity entity)
        => _context.Add(entity);

    public void Update(TEntity entity)
        => _context.Update(entity);

    public void Delete(TEntity entity)
        => _context.Remove(entity);

    public async Task<bool> SaveChangesAsync()
    {
        _context.OnBeforeSaveChanges();
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task StartTransactionAsync()
      => await _context.Database.BeginTransactionAsync();

    public async Task CommitTransactionAsync()
     => await _context.Database.CommitTransactionAsync();

    public async Task RollbackTransactionAsync()
      => await _context.Database.RollbackTransactionAsync();

    public void Dispose()
      => _context?.Dispose();
}
