using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{

    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly JujuTestContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(JujuTestContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll => _dbSet;

        public TEntity FindById(object id) => _dbSet.Find(id);

        public TEntity Create(TEntity entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public TEntity Update(TEntity editedEntity, TEntity originalEntity, out bool changed)
        {
            _context.Entry(originalEntity).CurrentValues.SetValues(editedEntity);
            changed = _context.Entry(originalEntity).State == EntityState.Modified;
            _context.SaveChanges();
            return originalEntity;
        }

        public TEntity Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
            return entity;
        }

        public void SaveChanges() => _context.SaveChanges();

        public JujuTestContext GetContext() => _context;

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }

}
